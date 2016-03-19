namespace TachographReader.Core
{
    using System;
    using Connect.Shared.Models;

    public interface IDocumentHistoryItem
    {
        string Type { get; set; }

        DateTime Created { get; set; }

        string DocumentType { get; set; }

        string RegistrationNumber { get; set; }

        string TechnicianName { get; set; }

        string Office { get; set; }

        string CustomerContact { get; set; }

        bool CanReprintLabel { get; set; }

        Document Document { get; set; }

        BaseReport Report { get; set; }

        GV212Report GV212Report { get; set; }

        void Print();
        void PrintLabel();

        void Email();

        bool IsReport();
        bool CanPrintGV212Document { get; set; }
    }
}