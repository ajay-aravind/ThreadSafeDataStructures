namespace ThreadSafeDS
{
    internal class ThreadSafeQueue<T>
    {
        private Node<T> head;
        private Node<T> tail;
        private object writeLock;

        public ThreadSafeQueue()
        {
            writeLock = new object();
        }

        public void Enqueue(T value)
        {
            lock (writeLock)
            {
                //Console.WriteLine("inside enqueue: value: " + value.ToString());
                //PrintList();
                Node<T> node = new Node<T>(value, null);
                if (tail != null)
                {
                    tail.next = node;
                    tail = node;
                }
                else
                {
                    head = node;
                    tail = node;
                }
            }
        }

        public T Dequeue()
        {
            lock (writeLock)
            {
                //Console.WriteLine("inside dequeue");
                //PrintList();
                if(head == null)
                {
                    throw new InvalidOperationException("queue is empty");
                }
                else
                {
                    T value = head.value;
                    head = head.next;
                    //Console.WriteLine("Dequeued: " + value.ToString());
                    return value;
                }
            }
        }

        public void PrintList()
        {
            Node<T> headCopy = head;

            while(headCopy != null)
            {
                Console.WriteLine(headCopy.value.ToString());
                headCopy= headCopy.next;
            }
        }
    }

    internal class Node<T>
    {
        internal T value;
        internal Node<T> next;

        public Node(T value, Node<T> next)
        {
            this.value = value;
            this.next = next;
        }
    }
}
