namespace SprintTracker2
{
    class Sprint
    {
        public Dictionary<int, Day> Days { get; } = new Dictionary<int, Day>();

        public Sprint()
        {
            
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
    }
    // Component
    abstract class TaskComponent
    {
        private int id;
        private string name;
        private TaskComponent parent;

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
        public TaskComponent GetParent()
        {
            return this.parent;
        }
        public void SetParent(TaskComponent parent)
        {
            this.parent = parent;
        }
        public abstract void Execute();
    }

    // Leaf
    class Task : TaskComponent
    {

        public Task(string name)
        {
            this.SetId();
            this.SetName(name); 
        }

        public override void Execute()
        {
            Console.WriteLine("Executing task " + this.GetId() + ": " + this.GetName());
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
        }

        public void RemoveChild(TaskComponent task)
        {
            subtasks.Remove(task);
        }

        public override void Execute()
        {
            Console.WriteLine("Executing task " + this.GetId() + ": " + this.GetName());
            foreach (var task in subtasks)
            {
                task.Execute();
            }
        }


        public void Iterate()
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
        }
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
        static void Main(string[] args)
        {
            /*// Depth 1
            TaskComposite rootTask = new TaskComposite();

            // Depth 2
            TaskComposite compositeTask1 = new TaskComposite();
            rootTask.AddTask(compositeTask1);

            TaskComponent taskA1 = new Task("Task A1");
            compositeTask1.AddTask(taskA1);

            TaskComponent taskB1 = new Task("Task B1");
            compositeTask1.AddTask(taskB1);

            // Depth 3
            TaskComposite compositeTask2 = new TaskComposite();
            compositeTask1.AddTask(compositeTask2);

            TaskComponent taskA2 = new Task("Task A2");
            compositeTask2.AddTask(taskA2);

            TaskComponent taskB2 = new Task("Task B2");
            compositeTask2.AddTask(taskB2);

            // Depth 4
            TaskComponent task1 = new Task("Task 1");
            compositeTask2.AddTask(task1);

            TaskComponent task2 = new Task("Task 2");
            compositeTask2.AddTask(task2);

            // Executing tasks
            rootTask.Execute();

            // Output:
            // Executing composite task:
            //   Executing composite task:
            //     Executing task: Task A1
            //     Executing task: Task B1
            //     Executing composite task:
            //       Executing task: Task A2
            //       Executing task: Task B2
            //       Executing task: Task 1
            //       Executing task: Task 2

            rootTask.IterateThroughChildren();*/

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
            Console.WriteLine($"Grand Child ID: {grandchild.GetId()}");
            Console.WriteLine($"Leaf Task ID: {leaf.GetId()}");


            Console.WriteLine("");
            // iterate
            root.Iterate();

            Day m = new Day(new DateOnly(2023,12,11));
            m.AddTask(root);
            Console.WriteLine(m.GetId());
            List<TaskComposite> tasklist = m.GetPrimaryTasks();
            foreach (TaskComposite t in tasklist)
            {
                Console.WriteLine(t.GetId() + t.GetName());
            }

            Console.ReadLine();
        }
    }
}