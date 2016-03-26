namespace TachographReader.Library.Web
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using DataModel.Repositories;
    using Properties;
    using Shared;
    using Shared.Helpers;
    using ViewModels;

    public static class StatusReport
    {
        private static Random _random = new Random();

        public static string Create(StatusReportViewModel viewModel)
        {
            var directory = Directory.CreateDirectory(Path.Combine(ImageHelper.GetTemporaryDirectory(), "Status Report " + DateTime.Now.Date.ToString("ddMMyyyy")));

            var resources = new[]
            {
                new {ResourceName = "bootstrap_css", FileName = "bootstrap.min.css"},
                new {ResourceName = "bootstrap_js", FileName = "bootstrap.min.js"},
                new {ResourceName = "chart_js", FileName = "Chart.min.js"},
                new {ResourceName = "gauge_js", FileName = "gauge.min.js"},
                new {ResourceName = "jquery_js", FileName = "jquery.min.js"},
                new {ResourceName = "style_css", FileName = "style.css"},
                new {ResourceName = "print_css", FileName = "print.css"},
            };

            foreach (var resource in resources)
            {
                var data = LocalizationHelper.GetResourceManager().GetObject(resource.ResourceName.Replace("-", "_"), CultureInfo.CurrentUICulture);
                if (data != null)
                {
                    File.WriteAllText(Path.Combine(directory.FullName, resource.FileName), data.ToString());
                }
            }

            var logo = ImageHelper.LoadFromResourcesAsByteArray("webcal");
            File.WriteAllBytes(Path.Combine(directory.FullName, "webcal.png"), logo);

            var path = Path.Combine(directory.FullName, "index.html");
            File.WriteAllText(path, GetMarkup(viewModel));
            return path;
        }

        private static string GetMarkup(StatusReportViewModel statusReport)
        {
            var workshopSettings = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();

            string tachoCentreQuarterlyStatusText;
            var tachoCentreQuarterlyStatusTextColor = GetStatus(statusReport.TachoCentreQuarterlyStatus, out tachoCentreQuarterlyStatusText);

            string gv212StatusText;
            var gv212StatusTextColor = GetStatus(statusReport.GV212Status, out gv212StatusText);

            var centreLastCheck = statusReport.TachoCentreLastCheck.GetValueOrDefault();

            var tachoRepository = ContainerBootstrapper.Resolve<TachographDocumentRepository>();
            var last12Months = GetLast12Months();
            var from = last12Months.First();
            var to = last12Months.Last().AddMonths(1).AddDays(-1);
            var documents = tachoRepository.Where(c => c.Created >= from && c.Created <= to)
                .Select(c => new ReportDocumentViewModel
                {
                    Technician = c.Technician,
                    Created = c.Created
                })
                .ToList();

            var technicians = statusReport.Technicians.Select(c => new TechnicianViewModel(documents, last12Months, c)).ToList();
            
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<!DOCTYPE html>" +
                                 "<html lang=\"en\">" +
                                 "<head>" +
                                 "    <meta charset=\"UTF-8\">" +
                                 "    <title>Webcal Status Report</title>" +
                                 "    <link href=\"bootstrap.min.css\" rel=\"stylesheet\">" +
                                 "    <link href=\"style.css\" rel=\"stylesheet\" />" +
                                 "    <link href=\"print.css\" media=\"print\" rel=\"stylesheet\" />" +
                                 "</head>" +
                                 "<body>" +
                                 "    <div class=\"container\">" +
                                 "        <div class=\"row\">" +
                                 "            <div class=\"col-xs-12\">" +
                                 "                <img src=\"webcal.png\" alt=\"Skillray Transport Services Ltd\" class=\"logo\" />" +
                                 "            </div>" +
                                 "        </div>" +
                                 "        <div class=\"row\">" +
                                 "            <div class=\"col-xs-6 contact-details section\">" +
                                 "                <address>");

            if (!string.IsNullOrEmpty(workshopSettings.WorkshopName))
                stringBuilder.AppendLine("                    <h2>" + workshopSettings.WorkshopName + "</h2>");
            if (!string.IsNullOrEmpty(workshopSettings.Address1))
                stringBuilder.AppendLine("                    " + workshopSettings.Address1 + ",<br/>");
            if (!string.IsNullOrEmpty(workshopSettings.Address2))
                stringBuilder.AppendLine("                    " + workshopSettings.Address2 + ",<br/>");
            if (!string.IsNullOrEmpty(workshopSettings.Address3))
                stringBuilder.AppendLine("                    " + workshopSettings.Address3 + ",<br/>");
            if (!string.IsNullOrEmpty(workshopSettings.Town))
                stringBuilder.AppendLine("                    " + workshopSettings.Town + ",<br/>");
            if (!string.IsNullOrEmpty(workshopSettings.PostCode))
                stringBuilder.AppendLine("                    " + workshopSettings.PostCode + ",<br/>");

            stringBuilder.AppendLine("                    <br/></br>");

            if (!string.IsNullOrEmpty(workshopSettings.PhoneNumber))
                stringBuilder.AppendLine("                    <strong>Phone: </strong>" + workshopSettings.PhoneNumber);

            stringBuilder.AppendLine("                </address>" +
                                 "            </div>" +
                                 "            <div class=\"col-xs-6 gauge section\">" +
                                 "                <h2>Overall Status</h2>" +
                                 "                <canvas id=\"overall-status\"></canvas>" +
                                 "            </div>" +
                                 "            <div class=\"col-xs-6 piechart section\">" +
                                 "                <h2>Technician Performance</h2>" +
                                 "                <div>" +
                                 "                    <canvas id=\"piechart\"></canvas>" +
                                 "                    <div id=\"js-legend\" class=\"chart-legend\"></div>" +
                                 "                </div>" +
                                 "            </div>" +
                                 "            <div class=\"col-xs-6 bar section\">" +
                                 "                <h2>Jobs Completed</h2>" +
                                 "                <canvas id=\"barchart\" height=\"230\" width=\"350\"></canvas>" +
                                 "            </div>" +
                                 "        </div>" +
                                 "        <div class=\"row checks\">" +
                                 "            <div class=\"col-xs-6\">" +
                                 "                <h3>Centre Quarterly Check</h3>" +
                                 "                <table class=\"table table-striped table-hover\">" +
                                 "                    <thead>" +
                                 "                        <tr>" +
                                 "                            <th>Date of last check</th>" +
                                 "                            <th>Date of next check</th>" +
                                 "                            <th>Status</th>" +
                                 "                        </tr>" +
                                 "                    </thead>" +
                                 "                    <tbody>" +
                                 "                        <tr>" +
                                 "                            <td><span style=\"color:#" + tachoCentreQuarterlyStatusTextColor + ";\">" + centreLastCheck.ToString(Constants.ShortYearDateFormat) + "</span></td>" +
                                 "                            <td><span style=\"color:#" + tachoCentreQuarterlyStatusTextColor + ";\">" + centreLastCheck.AddMonths(3).Date.ToString(Constants.ShortYearDateFormat) + "</span></td>" +
                                 "                            <td><span style=\"color:#" + tachoCentreQuarterlyStatusTextColor + ";\">" + tachoCentreQuarterlyStatusText + "</span></td>" +
                                 "                        </tr>" +
                                 "                    </tbody>" +
                                 "                </table>" +
                                 "            </div>" +
                                 "            <div class=\"col-xs-6\">" +
                                 "                <h3>Monthly GV212</h3>" +
                                 "                <table class=\"table table-striped table-hover\">" +
                                 "                    <thead>" +
                                 "                        <tr>" +
                                 "                            <th>Generated and printed</th>" +
                                 "                            <th>Status</th>" +
                                 "                        </tr>" +
                                 "                    </thead>" +
                                 "                    <tbody>" +
                                 "                        <tr>" +
                                 "                            <td><span style=\"color:#" + gv212StatusTextColor + ";\">" + statusReport.GV212LastCheck.GetValueOrDefault().ToString(Constants.ShortYearDateFormat) + "</span></td>" +
                                 "                            <td><span style=\"color:#" + gv212StatusTextColor + ";\">" + gv212StatusText + "</span></td>" +
                                 "                        </tr>" +
                                 "                    </tbody>" +
                                 "                </table>" +
                                 "            </div>" +
                                 "        </div>" +
                                 "        <div class=\"row\">" +
                                 "            <div class=\"col-xs-12\">" +
                                 "                <h3>Technicians Qu Report</h3>" +
                                 "                <table class=\"table table-striped table-hover\">" +
                                 "                    <thead>" +
                                 "                        <tr>" +
                                 "                            <th>Technician Name</th>" +
                                 "                            <th>Date of next check</th>" +
                                 "                            <th>Training Date</th>" +
                                 "                            <th>Status</th>" +
                                 "                        </tr>" +
                                 "                    </thead>" +
                                 "                    <tbody>");

            foreach (var technician in statusReport.Technicians)
            {
                string statusText;
                var color = GetStatus(technician.HalfYearStatus(), out statusText);

                string threeYearStatusText;
                var threeYearColor = GetStatus(technician.ThreeYearStatus(), out threeYearStatusText);

                string statusTextColor;
                var blah = GetStatusText(statusText, threeYearStatusText, out statusTextColor);

                stringBuilder.AppendLine("                        <tr>" +
                                     "                            <td><span style=\"color:#" + color + ";\">" + technician.Name + "</span></td>" +
                                     "                            <td><span style=\"color:#" + color + ";\">" + technician.DateOfLastCheck.GetValueOrDefault().ToString(Constants.ShortYearDateFormat) + "</span></td>" +
                                     "                            <td><span style=\"color:#" + threeYearColor + ";\">" + technician.DateOfLast3YearCheck.GetValueOrDefault().ToString(Constants.ShortYearDateFormat) + "</span></td>" +
                                     "                            <td><span style=\"color:#" + statusTextColor + ";\">" + blah + "</span></td>" +
                                     "                        </tr>");
            }

            stringBuilder.AppendLine("                    </tbody>" +
                                     "                </table>" +
                                     "            </div>" +
                                     "        </div>" +
                                     "    </div>" +
                                     "    <script src=\"jquery.min.js\"></script>" +
                                     "    <script src=\"bootstrap.min.js\"></script>" +
                                     "    <script type=\"text/javascript\" src=\"Chart.min.js\"></script>" +
                                     "    <script type=\"text/javascript\" src=\"gauge.min.js\"></script>" +
                                     "    <script>" +
                                     "        $(function () {" +
                                     "            var data = [");

            for (var i = 0; i < technicians.Count; i++)
            {
                var technician = technicians[i];
                stringBuilder.AppendLine("                {" +
                                         "                    value: " + technician.JobsDoneInLast12Months + "," +
                                         "                    color: \"" + GetColor(i) + "\"," +
                                         "                    label: \"" + technician.Technician.Name.ToTitleCase() + "\"" +
                                         "                },");
            }

            stringBuilder.AppendLine("                ];" +
                                     "            var ctx = document.getElementById(\"piechart\").getContext(\"2d\");" +
                                     "            var myChart = new Chart(ctx).Pie(data, {" +
                                     "                legendTemplate: \"<ul class=\\\"<%=name.toLowerCase()%>-legend\\\"><% for (var i=0; i<segments.length; i++){%><li><span style=\\\"background-color:<%=segments[i].fillColor%>\\\"></span><%if(segments[i].label){%><%=segments[i].label%> (<%=segments[i].value%>)<%}%></li><%}%></ul>\"," +
                                     "                title: {" +
                                     "                    text: 'Sales MTD'," +
                                     "                    subtitle: 'test'," +
                                     "                    position: 'top-center'" +
                                     "                }" +
                                     "            });" +
                                     "            document.getElementById('js-legend').innerHTML = myChart.generateLegend();" +
                                     "            var opts = {" +
                                     "                lines: 12," +
                                     "                angle: 0.15," +
                                     "                lineWidth: 0.44," +
                                     "                pointer: {" +
                                     "                    length: 0.9, " +
                                     "                    strokeWidth: 0.035," +
                                     "                    color: '#000000' " +
                                     "                }," +
                                     "                limitMax: 'false', " +
                                     "                colorStart: '" + GetColorForScore(statusReport) + "', " +
                                     "                colorStop: '" + GetColorForScore(statusReport) + "', " +
                                     "                strokeColor: '#E0E0E0', " +
                                     "                generateGradient: true" +
                                     "            };" +
                                     "            var target = document.getElementById('overall-status'); " +
                                     "            var gauge = new Gauge(target).setOptions(opts);" +
                                     "            gauge.maxValue = 100;" +
                                     "            gauge.animationSpeed = 32;" +
                                     "            gauge.set(" + CalculateScore(statusReport) + "); " +
                                     "            var data = {" +
                                     "                labels: [");

            foreach (var month in last12Months)
            {
                stringBuilder.AppendLine("\"" + month.ToString("MMM yy") + "\",");
            }

            stringBuilder.AppendLine("]," +
                                     "                datasets: [" +
                                     "                    {" +
                                     "                        label: \"Jobs Completed\"," +
                                     "                        fillColor: \"#09355C\"," +
                                     "                        strokeColor: \"rgba(220,220,220,0.8)\"," +
                                     "                        highlightFill: \"rgba(220,220,220,0.75)\"," +
                                     "                        highlightStroke: \"rgba(220,220,220,1)\"," +
                                     "                        data: [");

            foreach (var month in last12Months)
            {
                var count = technicians.Sum(technicianViewModel => technicianViewModel.JobsMonthByMonth.First(d => d.Key == month).Value);
                stringBuilder.Append(count + ",");
            }

            stringBuilder.AppendLine("]" +
                                     "                    }" +
                                     "                ]" +
                                     "            };" +
                                     "            var ctx = document.getElementById(\"barchart\").getContext(\"2d\");" +
                                     "            new Chart(ctx).Bar(data, {" +
                                     "                barShowStroke: false" +
                                     "            });" +
                                     "        });" +
                                     "    </script>" +
                                     "</body>" +
                                     "</html>");

            return stringBuilder.ToString();
        }

        private static string GetStatus(ReportItemStatus itemStatus, out string statusText)
        {
            Color color;
            switch (itemStatus)
            {
                case ReportItemStatus.Ok:
                    color = Color.FromArgb(0, 100, 0);
                    statusText = Resources.TXT_STATUS_REPORT_ok;
                    break;
                case ReportItemStatus.CheckDue:
                    color = Color.FromArgb(255, 140, 0);
                    statusText = Resources.TXT_STATUS_REPORT_CHECK_DUE;
                    break;
                case ReportItemStatus.Expired:
                    color = Color.FromArgb(178, 34, 34);
                    statusText = Resources.TXT_STATUS_REPORT_EXPIRED;
                    break;
                default:
                    color = Color.FromArgb(178, 34, 34);
                    statusText = Resources.TXT_STATUS_REPORT_UNKNOWN;
                    break;
            }
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private static string GetStatusText(string statusText, string threeYearStatusText, out string color)
        {
            Func<Color, string> toColor = col => col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2");

            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_ok)
            {
                color = toColor(Color.FromArgb(0, 100, 0));
                return Resources.TXT_STATUS_REPORT_ok;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_CHECK_DUE && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_EXPIRED && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_UNKNOWN && threeYearStatusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_EXPIRED && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            color = toColor(Color.FromArgb(0, 0, 0));
            return string.Empty;
        }

        private static List<DateTime> GetLast12Months()
        {
            var result = new List<DateTime>();
            var now = DateTime.Parse("01/" + DateTime.Now.Month + "/" + DateTime.Now.Year);

            for (var i = 11; i > 0; i--)
            {
                result.Add(now.AddMonths(i * -1));
            }

            result.Add(now);
            return result;
        }

        private static int CalculateScore(StatusReportViewModel statusReport)
        {
            var centreQuarterlyStatus = statusReport.TachoCentreQuarterlyStatus.GetInflatedScore();
            var gv212Status = statusReport.GV212Status.GetInflatedScore();

            int technicianScore = 0;
            foreach (var technician in statusReport.Technicians)
            {
                technicianScore += technician.HalfYearStatus().GetScore();
                technicianScore += technician.ThreeYearStatus().GetScore();
            }

            var technicianMaxScore = statusReport.Technicians.Count * 4;
            var maxScore = 10 + technicianMaxScore;
            var actualScore = centreQuarterlyStatus + gv212Status + technicianMaxScore;
            var scorePercentage = (100*maxScore)/actualScore;

            if (statusReport.Technicians.Any(c => c.ThreeYearStatus() == ReportItemStatus.Expired || c.ThreeYearStatus() == ReportItemStatus.Unknown))
            {
                if (scorePercentage > 75)
                {
                    return 75;
                }
            }

            return scorePercentage;
        }

        private static string GetColorForScore(StatusReportViewModel statusReport)
        {
            var score = CalculateScore(statusReport);
            if (score <= 50)
            {
                return "#E40213";
            }
            if (score > 50 && score <= 75)
            {
                return "#FF8C00";
            }
            if (score > 75)
            {
                return "#006400";
            }
            return "#E40213";
        }

        private static string GetColor(int index)
        {
            var basicColors = new[] {"#09355C", "#CBCBCB", "#E40213"};
            var basicColor = basicColors.ElementAtOrDefault(index);
            if (basicColor == null)
            {
                var color = $"#{_random.Next(0x1000000):X6}";
                return color;
            }
            return basicColor;
        }
    }
}