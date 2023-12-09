using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SprintTracker2
{
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

        /*public void PrintDayIdsAndDates()
        {
            foreach (var dayEntry in Days)
            {
                int dayId = dayEntry.Key;
                Day day = dayEntry.Value;
                string dayIdString = day.GetId();
                string dueDateString = day.GetDate().ToString("yyyyMMdd");

                Console.WriteLine($"Day ID: {dayId}, Due Date: {dueDateString}");
            }
        }*/
        public string GetStringDayIdsAndDates()
        {
            var results = "";
            foreach (var dayEntry in Days)
            {
                int dayId = dayEntry.Key;
                Day day = dayEntry.Value;

                string dayIdString = day.GetId();
                string dueDateString = day.GetDate().ToString("yyyyMMdd");

                results += $"Day ID: {dayId}, Due Date: {dueDateString} \n";
            }

            return results;
        }

        public List<string> GetStringListDayIdsAndDates()
        {
            var results = new List<string>();

            foreach (var dayEntry in Days)
            {
                int dayId = dayEntry.Key;
                Day day = dayEntry.Value;

                string dayIdString = day.GetId();
                string dueDateString = day.GetDate().ToString("yyyyMMdd");

                results.Add($"Day ID: {dayId}, Due Date: {dueDateString}");
            }

            return results;
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

    // Observer interface
    public interface IIssueObserver
    {
        string UpdateIssue(Issue issue, string attributeName, string updatedValue);
    }

    // Observable interface
    public interface IIssueObservable
    {
        string Subscribe(IIssueObserver observer);
        string SubscribeMultiple(List<IIssueObserver> observers);
        string Unsubscribe(IIssueObserver observer);
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
        /*public void DisplayTeamMembers()
        {
            Console.WriteLine($"Team: {this.name}, TeamId: {this.id}");
            foreach (var teamMember in this.members)
            {
                Console.WriteLine($"  Team Member: {teamMember.name}, MemberId: {teamMember.id}");
            }
        }*/
        public string GetStringTeamMembers()
        {
            var results = $"Team: {this.name}, TeamId: {this.id}\n";

            foreach (var teamMember in this.members)
            {
                results += $"  Team Member: {teamMember.name}, MemberId: {teamMember.id}\n";
            }

            return results;
        }
        public List<string> GetStringListTeamMembers()
        {
            var results = new List<string>();

            results.Add($"Team: {this.name}, TeamId: {this.id}");

            foreach (var teamMember in this.members)
            {
                results.Add($"  Team Member: {teamMember.name}, MemberId: {teamMember.id}");
            }

            return results;
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
        /*public void UpdateIssue(Issue issue, string attributeName, string updatedValue)
        {
            Console.WriteLine($"{this.GetName()} received update: {issue.GetName()}'s {attributeName} is changed to {updatedValue}");
        }*/
        public string UpdateIssue(Issue issue, string attributeName, string updatedValue)
        {
            return $"{this.GetName()} received update: {issue.GetName()}'s {attributeName} is changed to {updatedValue}";
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
        private TaskAbs parentTask;
        private List<IIssueObserver> subscribers = new List<IIssueObserver>();


        public Issue(string title, string desc, Status status, TaskAbs task)
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
        public void SetParentTask(TaskAbs t)
        {
            this.parentTask = t;
        }

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
        public string Unsubscribe(IIssueObserver observer)
        {
            if (subscribers.Contains(observer))
            {

                subscribers.Remove(observer);

                TeamMember unsubscribedMember = observer as TeamMember;

                if (unsubscribedMember != null)
                {
                    return $"{unsubscribedMember.name} was unsubscribed from {parentTask.GetAssignedMember().GetName()}'s issue report {GetName()}";
                }
            }

            return "";
        }

        private void NotifyObservers(string attributeName, string updatedValue)
        {
            foreach (var observer in subscribers)
            {
                observer.UpdateIssue(this, attributeName, updatedValue);
            }
        }

        /*public void SubscribeMultiple(List<IIssueObserver> observers)
        {
            foreach (var observer in observers)
            {
                Subscribe(observer);
            }
        }*/
        public string SubscribeMultiple(List<IIssueObserver> observers)
        {
            var results = "";

            foreach (var observer in observers)
            {
                results += Subscribe(observer);
            }

            return results;
        }

        public string Subscribe(IIssueObserver observer)
        {
            subscribers.Add(observer);

            if (observer is TeamMember teamMember)
            {
                return $"{teamMember.GetName()} subscribed to the issue: {GetName()}\n";
            }

            return "";
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
    public class TaskDecorator : TaskAbs
    {
        private TaskAbs task;

        public TaskDecorator(TaskAbs decoratedTask)
        {
            this.task = decoratedTask;
        }
        public override string Iterate()
        {
            return task.Iterate();
        }
        public TaskAbs GetCore()
        {
            return this.task;
        }
    }
    public class UrgentTaskDecorator : TaskDecorator
    {
        public UrgentTaskDecorator(TaskAbs decoratedTask) : base(decoratedTask)
        {

        }
    }
    public abstract class MeetingTaskDecorator : TaskDecorator
    {
        protected TimeOnly meetingTime;

        public MeetingTaskDecorator(TaskAbs decoratedTask) : base(decoratedTask)
        {
        }

        public MeetingTaskDecorator(TaskAbs decoratedTask, TimeOnly meetingTime) : base(decoratedTask)
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

        public SmallMeetingTaskDecorator(TaskAbs decoratedTask, TimeOnly meetingTime, List<TeamMember> members)
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

        public MassMeetingTaskDecorator(TaskAbs decoratedTask, TimeOnly meetingTime, List<Team> teams)
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
    public abstract class TaskAbs
    {
        private int id;
        private string name;
        private DateOnly dueDate;
        private TaskAbs? parent;
        private TeamMember assignedMember;
        public List<Issue> issues { get; set; } = new List<Issue>();
        private List<IIssueObserver> observers = new List<IIssueObserver>();

        public TaskAbs()
        {
        }

        public TaskAbs(TeamMember assignedPerson, string taskName, DateOnly dueDate)
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
        public TaskAbs? GetParent()
        {
            return this.parent;
        }
        public void SetParent(TaskAbs parent)
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
        //public abstract void Iterate();
        public abstract string Iterate();
    }

    // Leaf
    class Task : TaskAbs
    {

        public Task(TeamMember assignedPerson, string taskName, DateOnly dueDate)
        {
            this.SetId();
            this.SetName(taskName);
            this.SetAssignedMember(assignedPerson);
            this.SetDueDate(dueDate);
        }

        public override string Iterate()
        {
            return $"{GetId()}: {GetName()} (Task)";
        }
    }

    // Composite
    public class TaskComposite : TaskAbs
    {
        protected List<TaskAbs> subtasks = new List<TaskAbs>();

        public TaskComposite(TeamMember assignedPerson, string taskName, DateOnly dueDate)
        {
            this.SetId();
            this.SetName(taskName);
            this.SetAssignedMember(assignedPerson);
            this.SetDueDate(dueDate);
            this.subtasks = new List<TaskAbs>();
        }
        public TaskComposite(TeamMember assignedPerson, string taskName, DateOnly dueDate, List<TaskAbs> children)
        {
            this.SetId();
            this.SetName(taskName);
            this.SetAssignedMember(assignedPerson);
            this.SetDueDate(dueDate);
            this.subtasks = children;
        }

        public void AddChild(TaskAbs task)
        {
            //subtasks.Add(task);
            task.SetParent(this);
            subtasks.Add(task);
            task.SetId();
            /*child.Parent = this;
            Children.Add(child);*/

        }

        public void RemoveChild(TaskAbs task)
        {
            subtasks.Remove(task);
        }
        public override string Iterate()
        {
            var result = $"{GetId()} {GetName()} \n";

            foreach (var child in subtasks)
            {
                result += $"- {child.Iterate()} \n";
            }

            return result;
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

        public static int GenerateId(TaskAbs task)
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
        public static int GenerateChildId(TaskAbs parent)
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
            ////// ITERATION VIA CONSOLE
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

            ////// DECORATORS
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

            ////// STRINGIFIED ITERATION
            DateOnly date = new DateOnly(2023,12,9);
            // Create root composite
            TaskComposite root = new TaskComposite(joe, "Root", date);

            // Level 1 children
            Task childTask1 = new Task(joe, "Child Task 1", date);
            TaskComposite childComposite1 = new TaskComposite(joe, "Child Composite 1", date);

            // Level 2 children
            Task grandchildTask1 = new Task(joe, "Grandchild Task 1", date);
            TaskComposite grandchildComposite1 = new TaskComposite(joe, "Grandchild Composite 1", date);

            // Level 3 children 
            Task greatGrandchildTask1 = new Task(joe, "Great Grandchild Task 1", date);

            // Level 4 children
            Task greatGreatGrandchildTask1 = new Task(joe, "Great Great Grandchild Task 1", date);

            // Add child elements  
            root.AddChild(childTask1);
            root.AddChild(childComposite1);

            childComposite1.AddChild(grandchildTask1);
            childComposite1.AddChild(grandchildComposite1);

            grandchildComposite1.AddChild(greatGrandchildTask1);

            //greatGrandchildTask1.AddChild(greatGreatGrandchildTask1);

            string iterationResult = root.Iterate();

            Console.WriteLine(iterationResult);

            Console.WriteLine("TEAM METHODS");

            /*string sprintDays = sprint.PrintDayIdsAndDates();
            Console.WriteLine(sprintDays);*/

            Sprint sprint = new Sprint(new DateOnly(2023, 2, 1));

            string sprintDays = sprint.GetStringDayIdsAndDates();
            Console.WriteLine(sprintDays);

            List<string> sprintDaysList = sprint.GetStringListDayIdsAndDates();
            foreach (string day in sprintDaysList)
            {
                Console.WriteLine(day);
            }


            Team team2 = new Team(1, "Team Alpha");
            team2.AddTeamMember(new TeamMember(1, "John"));
            team2.AddTeamMember(new TeamMember(2, "Bob"));
            team2.AddTeamMember(new TeamMember(3, "Jen"));
            team2.AddTeamMember(new TeamMember(4, "Kim"));
            team2.AddTeamMember(new TeamMember(5, "Ernie"));

            string teamMembers = team2.GetStringTeamMembers();
            Console.WriteLine(teamMembers);

            List<string> membersList = team2.GetStringListTeamMembers();
            foreach (string member in membersList)
            {
                Console.WriteLine(member);
            }


            ////// ISSUE NOTIFICATIONS STRING RETURNS
            // Create task 
            Task t2 = new Task(tom, "Fix bug", new DateOnly(2023,12,9));

            // Create issue
            Issue issue = new Issue("Bug", "Crashes app", Issue.Status.New, t2);

            // Subscribe single observer
            string subscribeResult = issue.Subscribe(tom);
            Console.WriteLine(subscribeResult);

            // Subscribe multiple observers
            List<IIssueObserver> observers = new List<IIssueObserver> { tom, mary };
            string subscribeMultipleResult = issue.SubscribeMultiple(observers);
            Console.WriteLine(subscribeMultipleResult);

            // Update issue to notify observers 
            issue.SetName("Critical bug");
            issue.SetStatus(Issue.Status.FixInProgress);
            issue.SetDesc("zzzzzzzzzzzzzzzz");


            // Unsubscribe observer
            string unsubscribeResult = issue.Unsubscribe(tom);
            Console.WriteLine(unsubscribeResult);

            // Update issue again
            issue.SetStatus(Issue.Status.Resolved);

            Console.ReadLine();
        }

    }
}