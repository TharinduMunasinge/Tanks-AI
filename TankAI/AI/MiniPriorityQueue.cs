using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace TankAI.AI
{
    public class MinPriorityQueue<Key> : IEnumerable<Int32>
    where Key : IComparable<Key>
    {
        private int NMAX; //the maximum number of elements in the priority queue       
        private int N; //the number of elements in the priority queue
        private int[] pq; //the array of items in the priority queue, pq[i] is an integer that points to the index of keys[]
        private int[] qp; //inverse of pq - qp[pq[i]] = pq[qp[i]] = i
        private Key[] keys; //the array of values in the priority queue

        public MinPriorityQueue(int NMAX)
        {
            if (NMAX < 0) throw new Exception();
            this.NMAX = NMAX;
            //initialize the keys, pq, and pq arrays
            keys = new Key[NMAX + 1];
            pq = new int[NMAX + 1];
            qp = new int[NMAX + 1];
            //Initialize each value in qp to -1
            for (int i = 0; i <= NMAX; i++) qp[i] = -1;
        }

        //return true if the priority queue is empty
        public Boolean IsEmpty() { return N == 0; }

        //Check to see if i is an index in the priority queue
        public Boolean Contains(int i)
        {
            if (i < 0 || i >= NMAX) throw new Exception();
            return qp[i] != -1;
        }

        //how many elements are in the priority queue
        public int Size()
        {
            return N;
        }

        // insert an element into the priority queue
        public void Insert(int i, Key key)
        {
            //check to make sure the index isn't already in the priority queue
            if (i < 0 || i >= NMAX) throw new Exception();
            if (Contains(i)) throw new Exception("index is already in the priority queue");
            //increase the element count by 1
            N++;
            qp[i] = N;
            //Add i to pq array which will point to key at index i
            pq[N] = i;
            keys[i] = key;
            //Call swim to maintain the minimum property by moving it up
            Swim(N);
        }

        /*
        * An important function in maintaining the minimum priority queue.
        * To make sure a key is in the proper position we want to move it up the "tree" by comparing it
        * to the parent key, if it is less then exchange it with it's parent.  We keep doing this iteratively until
        * the parent of the key is less than the key we are swimming.
        * The parent of a key at index pq[i] is the key at index pq[i/2].  As a note i/2 is always rounded down.
        * */
        private void Swim(int k)
        {
            while (k > 1 && Greater(k / 2, k))
            {
                Exchange(k, k / 2);
                k = k / 2;
            }
        }

        /*
        * An important function in maintaining the minimum priority queue.
        * To make sure a key is in the proper position we may to move it down the "tree" by comparing it
        * to its right child, if it is greater then exchange it with the child.  
        * We keep doing this iteratively until the right child of the key is greater than the key we are sinking.
        * The child of a key at index pq[i] is the key at index pq[2i].  
        * */
        private void Sink(int k)
        {
            while (2 * k <= N)
            {
                int j = 2 * k;
                if (j < N && Greater(j, j + 1)) j++;
                if (!Greater(k, j)) break;
                Exchange(k, j);
                k = j;
            }
        }

        /*
        * Compare the keys at index pq[i] and index pq[j], return true
        * if key at index pq[i] is greater than at index pq[j]
        * */
        private Boolean Greater(int i, int j)
        {
            return keys[pq[i]].CompareTo(keys[pq[j]]) > 0;
        }

        //Exchange keys in the priority queue by exchanging their indexes in pq
        private void Exchange(int i, int j)
        {
            int swap = pq[i]; pq[i] = pq[j]; pq[j] = swap;
            qp[pq[i]] = i; qp[pq[j]] = j;
        }

        //The minimum key in the queue (keys) will always be at pq[1]
        public Key MinKey()
        {
            if (N == 0) throw new Exception("Priority queue underflow");
            return keys[pq[1]];
        }

        //Delete pq[1] and re-order the queue
        public int DeleteMin()
        {
            if (N == 0) throw new Exception("Priority queue underflow");
            //get a reference to the minimum
            int min = pq[1];
            //exchange pq[1] with pq[N] and subtract 1 from N
            Exchange(1, N--);
            //pq[N] is not the minimum value so push it down
            Sink(1);
            /*
            * mark the item in the queue as deleted by setting pq[min] to -1 and the corresponding
            * keys value (which is now at the end of keys[]) to default(key)
            * */
            qp[min] = -1;
            keys[pq[N + 1]] = default(Key);
            pq[N + 1] = -1;
            return min;
        }

        /*
        * Change the Key value at index i
        * */
        public void ChangeKey(int i, Key key)
        {
            if (i < 0 || i >= NMAX) throw new Exception();
            if (!Contains(i)) throw new Exception("index is not in the priority queue");
            //first, update the key value
            keys[i] = key;
            /*
            * in order to ensure the minimum key is at the top of the que first move the key
            * to the top of the queue and then move it down
            * */
            Swim(qp[i]);
            Sink(qp[i]);
        }

        //set key at index i to a lower value and reorder the queue
        public void DecreaseKey(int i, Key key)
        {
            if (i < 0 || i >= NMAX) throw new Exception();
            if (!Contains(i)) throw new Exception("index is not in the priority queue");
            if (keys[i].CompareTo(key) <= 0) throw new Exception("Calling decreaseKey() with given argument would not strictly decrease the key");
            keys[i] = key;
            Swim(qp[i]);
        }

        //set the key at index i to a higher value and reorder the queue
        public void IncreaseKey(int i, Key key)
        {
            if (i < 0 || i >= NMAX) throw new Exception();
            if (!Contains(i)) throw new Exception("index is not in the priority queue");
            if (keys[i].CompareTo(key) >= 0) throw new Exception("Calling increaseKey() with given argument would not strictly increase the key");
            keys[i] = key;
            Sink(qp[i]);
        }

        //delete the key at index i
        public void Delete(int i)
        {
            if (i < 0 || i >= NMAX) throw new Exception();
            if (!Contains(i)) throw new Exception("index is not in the priority queue");
            int index = qp[i];
            Exchange(index, N--);
            Swim(index);
            Sink(index);
            keys[i] = default(Key);
            qp[i] = -1;
        }

        /*
        * Implement a custom iterator
        * */
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<Int32> GetEnumerator()
        {
            MinPriorityQueue<Key> copy = new MinPriorityQueue<Key>(pq.Length - 1);

            for (int i = 1; i <= N; i++)
            {
                copy.Insert(pq[i], keys[pq[i]]);
            }
            for (int i = 1; i <= N; i++)
            {
                yield return copy.DeleteMin();
            }
        }
    }

}
