using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreadSafeDS
{
    public class ThreadSafeList: IEnumerable<int>
    {
        private object readLock;
        private object writeLock;

        private List<int> _data;

        public ThreadSafeList()
        {
            _data = new List<int>();
            writeLock = new object();
        }

        public void Add(int element){
            
            lock(writeLock){
                this.PrintList();
                // Console.WriteLine("in Add: Before lock..");
                _data.Add(element);
            }
        }

        public void Delete(int index){
            lock(writeLock){
                this.PrintList();
                // Console.WriteLine("in delete: Before lock..Trying to delete element at:"+index);
                _data.RemoveAt(index);
            }
        }

        public int GetElementAt(int targetIndex){
            lock(writeLock){
                this.PrintList();
                Console.WriteLine("In GetElementAt: Before get..");
                int index =0;
                foreach(int element in _data){
                    if(targetIndex == index){
                        return element;
                    }
                }

                throw new ArgumentOutOfRangeException("Index is out of range");
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new ThreadSafeListIterator(_data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// always call from locked scope
        /// </summary>
        private void PrintList()
        {
            /*Console.WriteLine("PrintingList..");
            foreach (int element in _data)
            {
                Console.Write(element+",");
            }
            Console.WriteLine("End");*/
        }
    }

    internal class ThreadSafeListIterator : IEnumerator<int>
    {
        private List<int> _data;
        private int _index = -1;
        public int Current
        {
            get
            {
                if (_index < _data.Count)
                {
                    return _data.ElementAt(_index);
                }
                throw new InvalidOperationException();
            }
        }

        public ThreadSafeListIterator(List<int> data)
        {
            _data = data;
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _data.Count;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
