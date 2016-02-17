namespace TachographReader.Views
{
    using System;
    using Connect.Shared.Models;
    using Core;
    using Shared;

    public class QC3MonthCheckViewModel : BaseNewDocumentViewModel
    {
        public QC3MonthCheckViewModel()
        {
            Document = new QCReport3Month();
        }

        public QCReport3Month Document { get; set; }

        public bool IsReadOnly { get; set; }

        public IRepository<QCReport3Month> Repository { get; set; }
        
        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            Repository = GetInstance<IRepository<QCReport3Month>>();
        }
        
        protected override void Add()
        {
            Document.Created = DateTime.Now;
            Repository.AddOrUpdate(Document);
        }

        protected override BaseReport GetReport()
        {
            return Document;
        }
    }
}