namespace test{

    using System;
    using System.Collections;
    using System.Collections.Generic; 
    public class ConcurrentList{

        public static void Main(string[] args){
            Console.WriteLine("hello world");
            ThreadSafeList safeList = new ThreadSafeList();
            int i=20, j=0;
            List<Task> tasks = new List<Task>();

            for(j;j<i;j++){
                Task t = new Task(() => { safeList.Add(j)});
                tasks.Add(t);
            }
        }
    }

    internal class ThreadSafeList{
        
        private object readLock;
        private object writeLock;

        private List<int> _data = new List<int>();

        public void Add(int element){
            Console.WriteLine("in Add: Before lock..");
            lock(writeLock){
                _data.Add(element);
            }
        }

        public void Delete(int index){
            Console.WriteLine("in delete: Before lock..")
            lock(writeLock){
                _data.RemoveAt(index);
            }
        }

        public int GetElementAt(int targetIndex){
            Console.WriteLine("In GetElementAt: Before get..");
            lock(writeLock){
                int index =0;
                foreach(int element in _data){
                    if(targetIndex == index){
                        return element;
                    }
                }
            }
        }
    }
}