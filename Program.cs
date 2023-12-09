using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SprintTracker2
{
    public class TaskBuilder
    {
        private TeamMember assignedMember;
        private string name;
        private DateOnly dueDate;
        private TaskComponent task;

        public TaskBuilder WithAssignedMember(TeamMember member)
        {
            this.assignedMember = member;
            return this;
        }

        public TaskBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public TaskBuilder WithDueDate(DateOnly date)
        {
            this.dueDate = date;
            return this;
        }

        public Task BuildTask()
        {
            return new Task(assignedMember, name, dueDate);
        }

        public TaskComposite BuildCompositeTask()
        {
            return new TaskComposite(assignedMember, name, dueDate);
        }

 /*       public TaskBuilder AddSubtask(TaskComponent subtask)
        {
            if (task is Task)
            {
                // Attempt conversion 
                task = ConvertToComposite();
            }

            // Check for null
            if (task != null)
            {
                ((TaskComposite)task).AddChild(subtask);
            }

            return this;
        }

        private TaskComposite ConvertToComposite()
        {
            // If task is Task, convert it to a TaskComposite,
            // copy over properties, return it
            if (task is Task t)
            {
                var copy = new TaskComposite(t.GetAssignedMember(),
                                             t.GetName(),
                                             t.GetDueDate());

                return copy;
            }

            // Otherwise return null
            return conversionSucceeded ? composite : null;
        }*/
        public TaskDecorator BuildUrgentTask(TaskComponent task)
        {
            return new UrgentTaskDecorator(task);
        }

        public MeetingTaskDecorator BuildSmallMeetingTask(TaskComponent task,
                                                           TimeOnly time,
                                                           List<TeamMember> attendees)
        {
            return new SmallMeetingTaskDecorator(task, time, attendees);
        }

        public MeetingTaskDecorator BuildMassMeetingTask(TaskComponent task,
                                                         TimeOnly time,
                                                         List<Team> teams)
        {
            return new MassMeetingTaskDecorator(task, time, teams);
        }
    }
    public class Sprint
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
    public class Day
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
        void SubscribeMultiple(List<IIssueObserver> observers);
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
        public List<TeamMember> GetTeamMembers()
        {
            return this.members;
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


        public Issue(string title, string desc, Status status, TaskComponent task)
        {
            this.name = title;
            this.desc = desc;
            this.currStatus = status;
            this.parentTask = task;
        }

        public string GetName()
        {
            return this.name;
        }

        /*public void SetName(string newName)
        {
            this.name = newName;
        }*/
        public void SetName(string newName)
        {
            string oldName = this.name; // store the old value for comparison
            this.name = newName;

            // Notify observers about the name change
            if (oldName != newName)
            {
                NotifyObservers("Name", newName);
            }
        }

        public string GetDesc()
        {
            return this.desc;
        }

        /*public void SetDesc(string newDesc)
        {
            this.desc = newDesc;
        }*/
        public void SetDesc(string newDesc)
        {
            string oldDesc = this.desc; // store the old value for comparison
            this.desc = newDesc;

            // Notify observers about the description change
            if (oldDesc != newDesc)
            {
                NotifyObservers("Description", newDesc);
            }
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
            if (observer is TeamMember teamMember)
            {
                Console.WriteLine($"{teamMember.GetName()} subscribed to the issue: {GetName()}");
            }
        }

        /*public void Unsubscribe(IIssueObserver observer)
        {
            subscribers.Remove(observer);
        }*/
        public void Unsubscribe(IIssueObserver observer)
        {
            if (subscribers.Contains(observer))
            {
                subscribers.Remove(observer);
                TeamMember unsubscribedMember = observer as TeamMember;

                if (unsubscribedMember != null)
                {
                    Console.WriteLine($"{unsubscribedMember.name} was unsubscribed from {parentTask.GetAssignedMember().GetName()}'s issue report {GetName()}");
                }
            }
        }


        private void NotifyObservers(string attributeName, string updatedValue)
        {
            foreach (var observer in subscribers)
            {
                observer.UpdateIssue(this, attributeName, updatedValue);
            }
        }

        public void SubscribeMultiple(List<IIssueObserver> observers)
        {
            foreach (var observer in observers)
            {
                Subscribe(observer);
            }
        }

        /*public void AlertAndUnsubscribeAll()
        {
            // Alert one last time that the issue is resolved
            NotifyObservers("Status", Status.Resolved.ToString());

            // Create a copy of the subscribers list
            var subscribersCopy = new List<IIssueObserver>(subscribers);

            // Unsubscribe all observers
            foreach (var subscriber in subscribersCopy)
            {
                if (subscriber is TeamMember teamMember)
                {
                    teamMember.Unsubscribe(this);
                }
            }
        }*/
    }
    public class TaskDecorator : TaskComponent
    {
        private TaskComponent task;

        public TaskDecorator(TaskComponent decoratedTask)
        {
            this.task = decoratedTask;
        }
        public override void Iterate()
        {
            task.Iterate();
        }
        public TaskComponent GetCore()
        {
            return this.task;
        }
    }
    public class UrgentTaskDecorator : TaskDecorator
    {
        public UrgentTaskDecorator(TaskComponent decoratedTask) : base(decoratedTask)
        {

        }
    }
    public abstract class MeetingTaskDecorator : TaskDecorator
    {
        protected TimeOnly meetingTime;

        public MeetingTaskDecorator(TaskComponent decoratedTask) : base(decoratedTask)
        {
        }

        public MeetingTaskDecorator(TaskComponent decoratedTask, TimeOnly meetingTime) : base(decoratedTask)
        {
            this.meetingTime = meetingTime;
        }

        public void SetMeetingTime(TimeOnly time)
        {
            this.meetingTime = time;
        }

        public TimeOnly GetMeetingTime()
        {
            return this.meetingTime;
        }
        public TeamMember GetHost()
        {
            return base.GetCore().GetAssignedMember();
        }

        public abstract List<TeamMember> GetAttendees();
    }
    public class SmallMeetingTaskDecorator : MeetingTaskDecorator
    {
        private List<TeamMember> attendees;

        public SmallMeetingTaskDecorator(TaskComponent decoratedTask, TimeOnly meetingTime, List<TeamMember> members)
            : base(decoratedTask, meetingTime)
        {
            this.attendees = members;
        }

        public override List<TeamMember> GetAttendees()
        {
            return this.attendees;
        }
    }
    public class MassMeetingTaskDecorator : MeetingTaskDecorator
    {
        private List<Team> attendeeTeams;

        public MassMeetingTaskDecorator(TaskComponent decoratedTask, TimeOnly meetingTime, List<Team> teams)
            : base(decoratedTask, meetingTime)
        {
            this.attendeeTeams = teams;
        }

        public override List<TeamMember> GetAttendees()
        {
            var allAttendees = new List<TeamMember>();
            foreach (var team in attendeeTeams)
            {
                allAttendees.AddRange(team.GetTeamMembers());
            }
            return allAttendees;
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
    public class Task : TaskComponent
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
            /////// RECURSIVE ITERATION TEST
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


            /////// ISSUE NOTIFICATION TESTS
            /*
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
            Task task = new Task(joe, "Coding Task", new DateOnly(2023, 12, 8));
            Console.WriteLine("Task " + task.GetId() + " " + task.GetName());

            // Joe creates an issue and alerts Tom and Jane
            Issue issue = new Issue("Bug", "Application crash", Issue.Status.New, task);

            List<IIssueObserver> sublist = new List<IIssueObserver> { tom, jane };

            issue.Subscribe(tom);
            issue.Subscribe(jane);
            issue.SubscribeMultiple(sublist);

            // Joe updates the issue's name
            issue.SetName("Critical Bug");

            // Joe updates the issue's description
            issue.SetDesc("wow problemo");

            // Joe updates the status to Updated
            issue.SetStatus(Issue.Status.Updated);

            // Joe updates the status to Resolved
            issue.SetStatus(Issue.Status.Resolved);

            // Unsubscribe all subscribers
            //issue.Unsubscribe(tom);
            //issue.Unsubscribe(jane);
            //issue.AlertAndUnsubscribeAll(); // same as doing "issue.SetStatus(Issue.Status.Resolved);"
*/

            /////// TASK DECORATORS TEST
            /*// Create a team member
            TeamMember joey = new TeamMember(1, "Joey");

            // Create a basic task
            Task task2 = new Task(joey, "Important Task", new DateOnly(2023, 12, 8));

            // Decorate the basic task with urgency
            UrgentTaskDecorator urgentTask = new UrgentTaskDecorator(task2);

            // Decorate the basic task with a small meeting
            TimeOnly meetingTime = new TimeOnly(14, 30); // 2:30 PM
            List<TeamMember> meetingAttendees = new List<TeamMember> { joe, tom, mary };
            SmallMeetingTaskDecorator smallMeetingTask = new SmallMeetingTaskDecorator(task2, meetingTime, meetingAttendees);

            // Decorate the basic task with a mass meeting
            //List<Team> attendeeTeams = new List<Team> { new Team(2, "Team A", new List<TeamMember> { joe }) };
            List<Team> attendeeTeams = new List<Team> { team };
            MassMeetingTaskDecorator massMeetingTask = new MassMeetingTaskDecorator(task2, meetingTime, attendeeTeams);

            // Test the iteration of the decorated tasks
            Console.WriteLine("Original Task:");
            task2.Iterate();

            Console.WriteLine("\nUrgent Task:");
            urgentTask.Iterate();

            Console.WriteLine("\nSmall Meeting Task:");
            smallMeetingTask.Iterate();
            Console.WriteLine("Host of the meeting: " + smallMeetingTask.GetHost().GetName());
            Console.WriteLine("Meeting Time: " + smallMeetingTask.GetMeetingTime());
            Console.WriteLine("Attendees: " + string.Join(", ", smallMeetingTask.GetAttendees().Select(a => a.GetName())));

            Console.WriteLine("\nMass Meeting Task:");
            massMeetingTask.Iterate();
            Console.WriteLine("Host of the meeting: " + massMeetingTask.GetHost().GetName());
            Console.WriteLine("Meeting Time: " + massMeetingTask.GetMeetingTime());
            Console.WriteLine("Attendees: " + string.Join(", ", massMeetingTask.GetAttendees().Select(a => a.GetName())));

            // Decorate the Small Meeting Task with urgency
            UrgentTaskDecorator urgentSmallMeetingTask = new UrgentTaskDecorator(smallMeetingTask);

            // Get the core task
            MeetingTaskDecorator coreTask = (MeetingTaskDecorator)urgentSmallMeetingTask.GetCore();

            // Test the iteration of the decorated tasks
            Console.WriteLine("\nUrgent Small Meeting Task:");
            urgentSmallMeetingTask.Iterate();
            Console.WriteLine("Host of the meeting: " + coreTask.GetHost().GetName());
            Console.WriteLine("Meeting Time: " + coreTask.GetMeetingTime());
            Console.WriteLine("Attendees: " + string.Join(", ", coreTask.GetAttendees().Select(a => a.GetName())));*/


            /////// TASK BUILDERS TEST
            // Create some test data
            TeamMember joe = new TeamMember(1, "Joe");
            DateOnly dueDate = new DateOnly(2023, 12, 10);
            TimeOnly meetingTime = new TimeOnly(10, 30);
            List<TeamMember> members = GetTestMembers();

            // Create builder
            TaskBuilder builder = new TaskBuilder();

            // Build plain task
            Task task = builder
                .WithAssignedMember(joe)
                .WithName("Do work")
                .WithDueDate(dueDate)
                .BuildTask();

            // Build and decorate as urgent  
            TaskDecorator urgentTask = builder
                .BuildUrgentTask(task);

            // Build and decorate with meeting
            MeetingTaskDecorator meetingTask = builder
                .BuildSmallMeetingTask(task, meetingTime, members);

            // Print out tasks
            task.Iterate();
            urgentTask.Iterate();
            meetingTask.Iterate();


            /*Task subtask1 = new TaskBuilder().BuildTask();

            Task subtask2 = new TaskBuilder().BuildTask();

            // Create main task
            Task mainTask = new TaskBuilder()
                            .AddSubtask(subtask1)
                            .AddSubtask(subtask2)
                            .BuildTask(); // is converted to TaskComposite

            mainTask.Iterate();*/

            Console.ReadLine();

            static List<TeamMember> GetTestMembers()
            {
                return new List<TeamMember>() {
                    new TeamMember(1, "Tom"),
                    new TeamMember(2, "Mary")
                };

            }
        }
    }
}