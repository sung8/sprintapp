using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SprintTracker2
{
    class Sprint
    {
        private DateOnly startDate;
        public Dictionary<int, Day> Days { get; } = new Dictionary<int, Day>();

        public Sprint(DateOnly startDate)
        {
            this.startDate = startDate;
            this.CalculateDays();
        }

        private void CalculateDays()
        {
            // Fixed sprint duration of 14 days
            for (int i = 1; i <= 14; i++)
            {
                DateOnly dueDate = startDate.AddDays(i - 1); // Subtract 1 to start from day 1
                Day day = new Day(dueDate);
                Days.Add(i, day); // Use the actual day id from the loop index
            }
        }

        public void PrintDayIdsAndDates()
        {
            foreach (var dayEntry in Days)
            {
                int dayId = dayEntry.Key;
                Day day = dayEntry.Value;
                string dayIdString = day.GetId();
                string dueDateString = day.GetDate().ToString("yyyyMMdd");

                Console.WriteLine($"Day ID: {dayId}, Due Date: {dueDateString}");
            }
        }

    }
    class Day
    {
        private string id;
        // either 
        // 1. date in YYYYMMDD format
        // 2. something 1 - 14
        // ... as key for hashmap in sprint

        private DateOnly dueDate;
        public List<TaskComposite> rootTasks { get; } = new List<TaskComposite>();

        public Day(DateOnly dueDate)
        {
            this.dueDate = dueDate;
            //this.id = dueDate.ToString("yyyyMMdd");
        }
        public void AddTask(TaskComposite task)
        {
            rootTasks.Add(task);
        }
        public string GetId()
        {
            return this.id;
        }
        public List<TaskComposite> GetPrimaryTasks()
        {
            return this.rootTasks;
        }
        public DateOnly GetDate()
        {
            return this.dueDate;
        }
    }
    /*public interface IObserver<T>
    {
        void Update(T data);
    }

    public interface IObservable<T>
    {
        void Subscribe(IObserver<T> observer);
        void Unsubscribe(IObserver<T> observer);
    }*/
    // Observer interface
    public interface IIssueObserver
    {
        void UpdateIssue(Issue issue, string attributeName, string updatedValue);
    }

    // Observable interface
    public interface IIssueObservable
    {
        void Subscribe(IIssueObserver observer);
        void Unsubscribe(IIssueObserver observer);
    }
    public class Team
    {
        private int id;
        private string name;

        private List<TeamMember> members;
        //private HashSet<TeamMember> members = new HashSet<TeamMember>();
        public Team(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.members = new List<TeamMember>();
        }

        public Team(int id, string name, List<TeamMember> members)
        {
            this.id = id;
            this.name = name;
            this.members = members;
        }
        public int GetId()
        {
            return this.id;
        }
        public string GetName()
        {
            return this.name;
        }
        public void AddTeamMember(TeamMember member)
        {
            members.Add(member);
            member.assignedTeam = this;
        }

        public void DisplayTeamMembers()
        {
            Console.WriteLine($"Team: {this.name}, TeamId: {this.id}");
            foreach (var teamMember in this.members)
            {
                Console.WriteLine($"  Team Member: {teamMember.name}, MemberId: {teamMember.id}");
            }
        }
        public bool IsTeamMember(TeamMember member)
        {
            return members.Contains(member);
        }
    }
    public class TeamMember : IIssueObserver
    {
        public int id { get; set; }
        public string name { get; set; }
        public Team assignedTeam { get; set; }

        public TeamMember(int memberId, string memberName)
        {
            this.id = memberId;
            this.name = memberName;
        }

        public TeamMember(int memberId, string memberName, Team team)
        {
            this.id = memberId;
            this.name = memberName;
            this.assignedTeam = team;
        }
        public string GetName()
        {
            return this.name;
        }
        public Team GetAssignedTeam()
        {
            return this.assignedTeam;
        }
        public void UpdateIssue(Issue issue, string attributeName, string updatedValue)
        {
            Console.WriteLine($"{this.GetName()} received update: {issue.GetName()}'s {attributeName} is changed to {updatedValue}");
        }

        // Implement Unsubscribe method from IIssueObserver
        public void Unsubscribe(IIssueObservable observable)
        {
            observable.Unsubscribe(this);
        }

    }

    public class Issue : IIssueObservable
    {
        public enum Status
        {
            New,
            Updated,
            FixInProgress,
            Resolved
        }

        private string name;
        private string desc;
        //private DateOnly dateRaised;
        private Status currStatus;
        private TaskComponent parentTask;
        private List<IIssueObserver> subscribers = new List<IIssueObserver>();


        public Issue(string title, string desc, Status status)
        {
            this.name = title;
            this.desc = desc;
            this.currStatus = status;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        public string GetDesc()
        {
            return this.desc;
        }

        public void SetDesc(string newDesc)
        {
            this.desc = newDesc;
        }

        public Status GetStatus()
        {
            return this.currStatus;
        }

        /*public void SetStatus(Status newStatus)
        {
            if (newStatus is Status.Resolved)
            {
                this.ResolveIssue();
            }
            this.currStatus = newStatus;
            //ex:
            //myIssue.SetStatus(Issue.Status.New);
        }*/
        public void SetParentTask(TaskComponent t)
        {
            this.parentTask = t;
        }

        public void AssignToTask(TaskComponent task)
        {
            /*this.parentTask = task;
            task.Subscribe(new TeamMember("Issue Raiser")); */
        }

        /*protected void ResolveIssue()
        {
            if (parentTask != null)
            {
                // Use a TeamMember object here if needed
                //parentTask.Unsubscribe(subscriber);
            }

        }*/

        public void SetStatus(Status newStatus)
        {
            if (newStatus == Status.Resolved)
            {
                // Create a copy of the subscribers list
                var subscribersCopy = new List<IIssueObserver>(subscribers);

                // Notify observers
                NotifyObservers("Status", newStatus.ToString());

                // Unsubscribe the team members
                foreach (var subscriber in subscribersCopy)
                {
                    if (subscriber is TeamMember teamMember)
                    {
                        teamMember.Unsubscribe(this);
                    }
                }
            }

            this.currStatus = newStatus;
        }



        public void Subscribe(IIssueObserver observer)
        {
            subscribers.Add(observer);
        }

        public void Unsubscribe(IIssueObserver observer)
        {
            subscribers.Remove(observer);
        }

        private void NotifyObservers(string attributeName, string updatedValue)
        {
            foreach (var observer in subscribers)
            {
                observer.UpdateIssue(this, attributeName, updatedValue);
            }
        }
    }
    class TaskDecorator : TaskComponent
    {
        TaskComponent task;

        public TaskDecorator(TaskComponent decoratedTask)
        {
            this.task = decoratedTask;
        }
        public override void Iterate()
        {

        }
    }

    // Component
    public abstract class TaskComponent
    {
        private int id;
        private string name;
        private DateOnly dueDate;
        private TaskComponent? parent;
        private TeamMember assignedMember;
        public List<Issue> issues { get; set; } = new List<Issue>();
        private List<IIssueObserver> observers = new List<IIssueObserver>();

        public TaskComponent()
        {
        }

        public TaskComponent(TeamMember assignedPerson, string taskName, DateOnly dueDate)
        {
            this.assignedMember = assignedPerson;
            this.name = taskName;
            this.dueDate = dueDate;
        }

        public int GetId()
        {
            return this.id;
        }
        public void SetId()
        {
            this.id = TaskIdGenerator.GenerateId(this);
        }
        public string GetName()
        {
            return this.name;
        }
        public void SetName(string newName)
        {
            this.name = newName;
        }
        public TaskComponent? GetParent()
        {
            return this.parent;
        }
        public void SetParent(TaskComponent parent)
        {
            this.parent = parent;
        }

        public void SetDueDate(DateOnly date)
        {
            this.dueDate = date;
        }
        public DateOnly GetDueDate()
        {
            return this.dueDate;
        }
        public void SetAssignedMember(TeamMember assignedPerson)
        {
            this.assignedMember = assignedPerson;
        }
        public TeamMember GetAssignedMember()
        {
            return this.assignedMember;
        }

        public void AddIssue(Issue newIssueReport)
        {
            this.issues.Add(newIssueReport);
            newIssueReport.SetParentTask(this);
        }
        public void AddIssue(Issue newIssueReport, List<IIssueObserver> members)
        {
            this.issues.Add(newIssueReport);
            newIssueReport.SetParentTask(this);

            foreach (var member in members)
            {
                observers.Add(member);
            }
        }

        public List<Issue> GetAllIssues()
        {
            // Check if the task has issues
            if (this.issues == null || this.issues.Count == 0)
            {
                // No issues, return an empty list or null based on your preference
                return new List<Issue>();  // or return null;
            }

            // Return the list of issues
            return this.issues;
        }
        public void Subscribe(IIssueObserver observer)
        {
            observers.Add(observer);
        }

        public void Unsubscribe(IIssueObserver observer)
        {
            observers.Remove(observer);
        }

        private void NotifyObservers(string attributeName, string updatedValue)
        {
            foreach (var observer in observers)
            {
                observer.UpdateIssue(GetAllIssues().Last(), attributeName, updatedValue);
            }
        }

        //public abstract void Execute();
        public abstract void Iterate();
    }

    // Leaf
    class Task : TaskComponent
    {

        public Task(TeamMember assignedPerson, string taskName, DateOnly dueDate)
        {
            this.SetId();
            this.SetName(taskName);
            this.SetAssignedMember(assignedPerson);
            this.SetDueDate(dueDate);
        }

        /*public override void Execute()
        {
            Console.WriteLine("Executing task " + this.GetId() + ": " + this.GetName());
        }*/
        public override void Iterate()
        {
            Console.WriteLine($"{GetId()}: {GetName()} (Task)");
        }
    }

    // Composite
    public class TaskComposite : TaskComponent
    {
        protected List<TaskComponent> subtasks = new List<TaskComponent>();

        public TaskComposite(TeamMember assignedPerson, string taskName, DateOnly dueDate)
        {
            this.SetId();
            this.SetName(taskName);
            this.SetAssignedMember(assignedPerson);
            this.SetDueDate(dueDate);
            this.subtasks = new List<TaskComponent>();
        }
        public TaskComposite(TeamMember assignedPerson, string taskName, DateOnly dueDate, List<TaskComponent> children)
        {
            this.SetId();
            this.SetName(taskName);
            this.SetAssignedMember(assignedPerson);
            this.SetDueDate(dueDate);
            this.subtasks = children;
        }

        public void AddChild(TaskComponent task)
        {
            //subtasks.Add(task);
            task.SetParent(this);
            subtasks.Add(task);
            task.SetId();
            /*child.Parent = this;
            Children.Add(child);*/

        }

        public void RemoveChild(TaskComponent task)
        {
            subtasks.Remove(task);
        }

        /*public override void Execute()
        {
            Console.WriteLine("Executing task " + this.GetId() + ": " + this.GetName());
            foreach (var task in subtasks)
            {
                task.Execute();
            }
        }*/
        public override void Iterate()
        {
            Console.WriteLine(GetId() + ": " + GetName());

            foreach (var child in subtasks)
            {
                child.Iterate();
            }
        }


    }
    internal class TaskIdGenerator
    {
        private static int rootTaskCounter = 0;
        private static Dictionary<int, int> childTaskCounters = new Dictionary<int, int>();

        public static int GenerateRootId()
        {
            return ++rootTaskCounter;
            //return rootTaskCounter++;
        }

        public static int GenerateId(TaskComponent task)
        {
            if (task.GetParent() == null)
            {
                // This is a root node
                return GenerateRootId();

            }
            else
            {
                // This is a child node
                return GenerateChildId(task.GetParent());
            }
        }
        public static int GenerateChildId(TaskComponent parent)
        {

            if (!childTaskCounters.ContainsKey(parent.GetId()))
            {
                childTaskCounters[parent.GetId()] = 1;
            }

            int childId = parent.GetId() * 100 + childTaskCounters[parent.GetId()]++;

            return childId;

        }

    }

    public class Program
    {
        static void Main(string[] args)
        {
            /*// root 
            var root = new TaskComposite("Root");

            // child1 - Task 
            var child1 = new Task("Child Task 1");
            root.AddChild(child1);

            // child2 - TaskComposite
            var child2 = new TaskComposite("Child Composite 1");
            root.AddChild(child2);

            // grandchild 
            var grandchild1 = new Task("Grandchild 1 of Child Composite 1");
            child2.AddChild(grandchild1);

            var grandchild2 = new TaskComposite("Composite Grandchild 2 of Child Composite 1");
            child2.AddChild(grandchild2);

            // grandgrandchild1
            var grandgrandchild1 = new Task("Grand-Grandchild 1 of Composite Grandchild 2");
            grandchild2.AddChild(grandgrandchild1);

            // child3 - Task 
            var child3 = new Task("Child Task 2");
            root.AddChild(child3);


            // Iterate 
            root.Iterate();*/

            // Create team members
            TeamMember joe = new TeamMember(1, "Joe");
            TeamMember tom = new TeamMember(2, "Tom");
            TeamMember mary = new TeamMember(3, "Mary");
            TeamMember jane = new TeamMember(4, "Jane");

            // Create a team and add team members
            Team team = new Team(1, "Development Team");
            team.AddTeamMember(joe);
            team.AddTeamMember(tom);
            team.AddTeamMember(mary);
            team.AddTeamMember(jane);

            // Create a task assigned to Joe
            Task task = new Task(joe, "Task 1", new DateOnly(2023, 12, 8));

            // Joe creates an issue and alerts Tom and Jane
            Issue issue = new Issue("Bug", "Application crash", Issue.Status.New);
            issue.Subscribe(tom);
            issue.Subscribe(jane);

            // Joe updates the issue's name
            issue.SetName("Critical Bug");

            // Joe updates the status to Updated
            issue.SetStatus(Issue.Status.Updated);

            // Joe updates the status to Resolved
            issue.SetStatus(Issue.Status.Resolved);

            // Unsubscribe all subscribers
            issue.Unsubscribe(tom);
            issue.Unsubscribe(jane);

            Console.ReadLine();
        }

    }
}