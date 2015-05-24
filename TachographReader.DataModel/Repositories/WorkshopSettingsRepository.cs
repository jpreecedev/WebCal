namespace TachographReader.DataModel.Repositories
{
    using System.Data.Entity;
    using Microsoft.Practices.ObjectBuilder2;
    using Shared;

    public class WorkshopSettingsRepository : SettingsRepository<WorkshopSettings>
    {
        public override void Save(WorkshopSettings settings)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    //Bin old data, don't need it
                    context.CustomDayOfWeeks.RemoveRange(context.CustomDayOfWeeks);
                    context.SaveChanges();

                    var daysOfWeek = settings.CustomDayOfWeeks.Clone();
                    
                    //Save settings
                    settings.CustomDayOfWeeks = null;
                    context.Set<WorkshopSettings>().Attach(settings);
                    context.Entry(settings).State = EntityState.Modified;
                    context.SaveChanges();

                    //Save days of week
                    daysOfWeek.ForEach(c => c.WorkshopSettings = settings);
                    context.CustomDayOfWeeks.AddRange(daysOfWeek);

                    context.SaveChanges();
                }
            });
        }
    }
}