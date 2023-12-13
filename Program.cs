using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

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
                //string dayIdString = day.GetId();
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
    public interface IObserver<T>
    {
        void Update(T data);
    }

    public interface IObservable<T>
    {
        void Subscribe(IObserver<T> observer);
        void Unsubscribe(IObserver<T> observer);
    }
    public class Team
    {
        public int id { get; set; }
        public string name { get; set; }

        public List<TeamMember> members { get; set; }
        //private HashSet<TeamMember> members = new HashSet<TeamMember>();
        public Team(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public Team(int id, string name, List<TeamMember> members)
        {
            this.id = id;
            this.name = name;
            this.members = members;
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
    public class TeamMember : IObserver<string>
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
        public void Update(string data)
        {
            Console.WriteLine($"Team member '{name}' received update: {data}");
        }
    }

    class Issue
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

        public void SetStatus(Status newStatus)
        {
            if (newStatus is Status.Resolved)
            {
                this.ResolveIssue();
            }
            this.currStatus = newStatus;
            //ex:
            //myIssue.SetStatus(Issue.Status.New);
        }
        public void SetParentTask(TaskComponent t)
        {
            this.parentTask = t;
        }

        public void AssignToTask(TaskComponent task)
        {
            /*this.parentTask = task;
            task.Subscribe(new TeamMember("Issue Raiser")); */
        }

        protected void ResolveIssue()
        {
            if (parentTask != null)
            {
                // Use a TeamMember object here if needed
                //parentTask.Unsubscribe(subscriber);
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
    abstract class TaskComponent : IObservable<string>
    {
        private int id;
        private string name;
        private TaskComponent? parent;
        private TeamMember assignedMember;
        public List<Issue> issues { get; set; } = new List<Issue>();
        private List<IObserver<string>> observers = new List<IObserver<string>>();

        public TaskComponent()
        {
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
        public void AddIssue(Issue newIssueReport)
        {
            this.issues.Add(newIssueReport);
            newIssueReport.SetParentTask(this);
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

        public void Subscribe(IObserver<string> observer)
        {
            observers.Add(observer);
        }

        public void Unsubscribe(IObserver<string> observer)
        {
            observers.Remove(observer);
        }
        private void NotifyObservers(string data)
        {
            foreach (var observer in observers)
            {
                observer.Update(data);
            }
        }

        //public abstract void Execute();
        public abstract void Iterate();
    }

    // Leaf
    class Task : TaskComponent
    {

        public Task(string name)
        {
            this.SetId();
            this.SetName(name);
        }

        /*public override void Execute()
        {
            Console.WriteLine("Executing task " + this.GetId() + ": " + this.GetName());
        }*/
        public override void Iterate()
        {
            // Tasks have no children - do nothing
        }
    }

    // Composite
    class TaskComposite : TaskComponent
    {
        protected List<TaskComponent> subtasks = new List<TaskComponent>();

        public TaskComposite(string name)
        {
            this.SetId();
            this.SetName(name);
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

        public List<TaskComponent> GetSubtasks()
        {
            return subtasks;
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

        /*public void Iterate()
        {
            Console.WriteLine("Iterating through all children:");
            foreach (var child in subtasks)
            {
                // Only iterate through the children, not ourselves (root task)
                if (child != this)
                {
                    Visit(child);
                }
            }
        }

        private void Visit(TaskComponent task)
        {
            task.Execute();
            if (task is TaskComposite composite)
            {
                foreach (var child in composite.subtasks)
                {
                    if (child != composite)
                    {
                        Visit(child);
                    }
                }
            }
        }*/
        /*private void Visit(TaskComponent task, HashSet<int> visitedTasks)
        {
            if (visitedTasks.Contains(task.GetId()))
            {
                // Skip already visited tasks to avoid duplicates
                return;
            }

            visitedTasks.Add(task.GetId());
            task.Execute();

            if (task is TaskComposite composite)
            {
                foreach (var child in composite.subtasks)
                {
                    if (child != composite)
                    {
                        Visit(child, visitedTasks);
                    }
                }
            }
        }

        public void Iterate()
        {
            Console.WriteLine("Iterating through all children:");
            var visitedTasks = new HashSet<int>();
            foreach (var child in subtasks)
            {
                // Only iterate through the children, not ourselves (root task)
                if (child != this)
                {
                    Visit(child, visitedTasks);
                }
            }
        }*/

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

    internal class Program
    {
        public static void GraphTask(TaskComposite rootTask)
        {

            // Create the Windows Form and viewer
            Form form = new Form();
            GViewer viewer = new GViewer();

            // Create the graph
            Graph graph = new Graph("task");

            // Recursive helper method to add nodes and edges
            AddTaskToGraph(rootTask, graph);

            // Bind and display the graph
            viewer.Graph = graph;
            form.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            form.ShowDialog();

        }

        public static void AddTaskToGraph(TaskComposite task, Graph graph)
        {
            // Add node for this task with both ID and name
            string nodeId = $" {task.GetId()}\n {task.GetName()} ";
            graph.AddNode(nodeId);

            List<TaskComponent> temp = task.GetSubtasks();

            // Recursively add child tasks
            foreach (TaskComponent childTask in temp)
            {
                // Add edge between parent and child
                string childNodeId = $" {childTask.GetId()}\n {childTask.GetName()} ";
                graph.AddEdge(nodeId, childNodeId);

                // Recursively add child tasks
                if (childTask is TaskComposite)
                {
                    AddTaskToGraph((TaskComposite)childTask, graph);
                }
            }
        }
        static void Main(string[] args)
        {
            // root
            TaskComposite root = new TaskComposite("Root");
            //root.SetId();

            // child 
            TaskComposite child = new TaskComposite("Child");
            root.AddChild(child);
            //child.SetId();
            TaskComposite child2 = new TaskComposite("Child2");
            root.AddChild(child2);

            // grandchild
            TaskComposite grandchild = new TaskComposite("Grand Child");
            child.AddChild(grandchild);
            //grandchild.SetId();
            TaskComposite grandchild2 = new TaskComposite("Grand Child2");
            child.AddChild(grandchild2);

            TaskComposite grandchild3 = new TaskComposite("Grand Child3");
            child2.AddChild(grandchild3);


            // more leaf 
            TaskComposite leaf = new TaskComposite("Leaf");
            grandchild.AddChild(leaf);
            //leaf.SetId();

            TaskComposite leaf2 = new TaskComposite("Leaf2");
            grandchild3.AddChild(leaf2);
            // Print out IDs
            Console.WriteLine($"Root ID: {root.GetId()}");
            Console.WriteLine($"Child ID: {child.GetId()}");
            Console.WriteLine($"Child ID: {child2.GetId()}");
            Console.WriteLine($"Grand Child ID: {grandchild.GetId()}");
            Console.WriteLine($"Grand Child ID: {grandchild2.GetId()}");
            Console.WriteLine($"Grand Child ID: {grandchild3.GetId()}");
            Console.WriteLine($"Leaf Task ID: {leaf.GetId()}");
            Console.WriteLine($"Leaf Task ID: {leaf2.GetId()}");

            Console.WriteLine("Check parent");
            TaskComponent? parentTask = root.GetParent();
            if (parentTask != null)
            {
                // Handle the case where the task has a parent
                Console.WriteLine(parentTask.GetId());
            }
            else
            {
                // Handle the case where the task is a root-level task (no parent)
                Console.WriteLine("Root task has no parent.");
            }
            //Console.WriteLine(root.GetParent().GetId()); // null error
            Console.WriteLine(child.GetParent().GetId());
            Console.WriteLine(child2.GetParent().GetId());
            Console.WriteLine(grandchild.GetParent().GetId());
            Console.WriteLine(grandchild3.GetParent().GetId());
            Console.WriteLine(leaf.GetParent().GetId());
            Console.WriteLine(leaf2.GetParent().GetId());

            Console.WriteLine("root");
            root.Iterate();

            Console.WriteLine("\nchild2");
            child2.Iterate();

            Day m = new Day(new DateOnly(2023, 12, 4));
            m.AddTask(root);
            Console.WriteLine(m.GetId());
            List<TaskComposite> tasklist = m.GetPrimaryTasks();
            foreach (TaskComposite t in tasklist)
            {
                Console.WriteLine(t.GetId() + t.GetName());
            }

            Sprint s = new Sprint(new DateOnly(2023, 11, 29));
            s.PrintDayIdsAndDates();

            Console.WriteLine("\nIssue");
            Issue i1 = new Issue("Blocker", "cannot progess", Issue.Status.Resolved);
            Issue i2 = new Issue("Deprecated Dependencies", "desc", Issue.Status.Updated);
            Issue i3 = new Issue("Blocker 2", "desc", Issue.Status.New);

            child.AddIssue(i1);
            child.AddIssue(i2);
            child.AddIssue(i3);

            List<Issue> li = child.GetAllIssues();
            foreach (Issue i in li)
            {
                Console.WriteLine("Task " + child.GetId() + "'s issue " + i.GetName() + ", " + i.GetStatus().ToString() + ", " + i.GetDesc());
            }

            GraphTask(root);

            /*//create a form 
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            //create the graph content 
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
            Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.ShowDialog();*/

            Console.ReadLine();
        }
    }

}