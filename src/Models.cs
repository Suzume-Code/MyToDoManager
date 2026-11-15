using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PersonalTodo
{
    public enum ItemKind
    {
        Todo,
        Memo
    }

    public enum TodoStatus
    {
        New,
        InProgress,
        OnHold,
        Waiting,
        Completed
    }

    public enum Priority
    {
        Low,
        Normal,
        High
    }

    [Serializable]
    public class WorkItem
    {
        public string Id { get; set; }
        public ItemKind Kind { get; set; }
        public string Title { get; set; }
        public string MailUrl { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public TodoStatus Status { get; set; }
        public Priority Priority { get; set; }

        public WorkItem()
        {
            Id = Guid.NewGuid().ToString("N");
            Kind = ItemKind.Todo;
            Title = "";
            MailUrl = "";
            Body = "";
            CreatedAt = DateTime.Now;
            DueDate = DateTime.Today.AddDays(7);
            Status = TodoStatus.New;
            Priority = Priority.Normal;
        }
    }

    [Serializable]
    public class FolderBookmark
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Note { get; set; }

        public FolderBookmark()
        {
            Id = Guid.NewGuid().ToString("N");
            Name = "";
            Path = "";
            Note = "";
        }
    }

    [Serializable]
    [XmlRoot("PersonalTodoData")]
    public class AppData
    {
        public List<WorkItem> Items { get; set; }
        public List<FolderBookmark> Folders { get; set; }

        public AppData()
        {
            Items = new List<WorkItem>();
            Folders = new List<FolderBookmark>();
        }
    }
}