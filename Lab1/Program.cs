using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab1
{
    delegate void TaskDelegate();

    class Program
    {
        public static void WriteHello()
        {
            Console.WriteLine("Hello world!");
        }
        public static void WriteSum4_5()
        {
            Console.WriteLine(4+5);
        }
        public static void WriteForm()
        {
            Console.WriteLine("751001");
        }
        public static void WriteTB()
        {
            Console.WriteLine("Tatyana Bondareva");
        }  
       static void Main(string[] args)
        {
            Console.WriteLine("Введите количество потоков");
            
            int Count = int.Parse(Console.ReadLine());
            TaskQueue task = new TaskQueue(Count);
            Random rnd = new Random();
            for (int i=0; i<1000; i++){
                int value = rnd.Next(0, 4);
                switch (value)
                {
                    case 0: 
                        task.EnqueueTask(WriteHello);
                        break;
                    case 1:
                        task.EnqueueTask(WriteSum4_5);
                        break;
                    case 2:
                        task.EnqueueTask(WriteForm);
                        break;
                    case 3:
                        task.EnqueueTask(WriteTB);
                        break;
                }
                
            }
            task.IsStop = true;
            Console.ReadKey();
        }
    }
    
    class TaskQueue
    {

        static object locker = new object();
        static Queue<TaskDelegate> QTask;
        private  Thread[] threads;
        private volatile bool isStop=false;
        public  bool IsStop
        {
            get { return isStop; }
            set { isStop = value; }
        }

        public TaskQueue(int threadCount)//создаем пул потоков
        {
            QTask = new Queue<TaskDelegate>();
            threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(Check);
                thread.Start();
            }

        }
        public bool EnqueueTask(TaskDelegate task)
        {
            if (isStop)
                return false;
            QTask.Enqueue(task);
            return true;
        }


        public  void Check()//проверяем, есть ли в очереди задачи
        {
            while (!isStop || (QTask.Count > 0))
            {//если еще не было конца или очередь не пуста
                TaskDelegate task = null;
                lock (locker)//блокировка, на время обращения к очереди
                {
                    if (QTask.Count > 0)
                        task = QTask.Dequeue();
                }
                    task?.Invoke();//вызов задачи 
            }
            
        }
    }
}
