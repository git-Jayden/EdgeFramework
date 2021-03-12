/****************************************************
	�ļ���BinaryHeap.cs
	Author��JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date��2021/03/11 11:27   	
	Features��
*****************************************************/
using System;


namespace EdgeFramework
{
    using System;

    public enum BinaryHeapBuildMode
    {
        kNLog = 1,
        kN = 2,
    }

    public enum BinaryHeapSortMode
    {
        kMin = 0,
        kMax = 1,
    }
    //���ȶ���&�����
    public class BinaryHeap<T> where T : IBinaryHeapElement
    {
        protected T[] mArray;

        protected float mGrowthFactor = 1.6f;
        protected int mLastChildIndex; //����ӽڵ��λ��
        protected BinaryHeapSortMode mSortMode;

        public BinaryHeap(int minSize, BinaryHeapSortMode sortMode)
        {
            mSortMode = sortMode;
            mArray = new T[minSize];
            mLastChildIndex = 0;
        }

        public BinaryHeap(T[] dataArray, BinaryHeapSortMode sortMode)
        {
            mSortMode = sortMode;
            int minSize = 10;
            if (dataArray != null)
            {
                minSize = dataArray.Length + 1;
            }

            mArray = new T[minSize];
            mLastChildIndex = 0;
            Insert(dataArray, BinaryHeapBuildMode.kN);
        }

        #region ��������

        #region ���

        public void Clear()
        {
            mArray = new T[10];
            mLastChildIndex = 0;
        }

        #endregion

        #region ����

        public void Insert(T[] dataArray, BinaryHeapBuildMode buildMode)
        {
            if (dataArray == null)
            {
                throw new NullReferenceException("BinaryHeap Not Support Insert Null Object");
            }

            int totalLength = mLastChildIndex + dataArray.Length + 1;
            if (mArray.Length < totalLength)
            {
                ResizeArray(totalLength);
            }

            if (buildMode == BinaryHeapBuildMode.kNLog)
            {
                //��ʽ1:ֱ����ӣ�ÿ����Ӷ����ϸ�
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    Insert(dataArray[i]);
                }
            }
            else
            {
                //�����Ƚϴ������»��һЩ
                //��ʽ2:������꣬Ȼ������
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    mArray[++mLastChildIndex] = dataArray[i];
                }

                SortAsCurrentMode();
            }
        }

        public void Insert(T element)
        {
            if (element == null)
            {
                throw new NullReferenceException("BinaryHeap Not Support Insert Null Object");
            }

            int index = ++mLastChildIndex;

            if (index == mArray.Length)
            {
                ResizeArray();
            }

            mArray[index] = element;

            ProcolateUp(index);
        }

        #endregion

        #region ����

        public T Pop()
        {
            if (mLastChildIndex < 1)
            {
                return default(T);
            }

            T result = mArray[1];
            mArray[1] = mArray[mLastChildIndex--];
            ProcolateDown(1);
            return result;
        }

        public T Top()
        {
            if (mLastChildIndex < 1)
            {
                return default(T);
            }

            return mArray[1];
        }

        #endregion

        #region ��������

        public void Sort(BinaryHeapSortMode sortMode)
        {
            if (mSortMode == sortMode)
            {
                return;
            }
            mSortMode = sortMode;
            SortAsCurrentMode();
        }

        public void RebuildAtIndex(int index)
        {
            if (index > mLastChildIndex)
            {
                return;
            }

            //1.�����Ҹ��ڵ㣬�Ƿ�ȸ��ڵ�С������������ϸ�,�������³�
            var element = mArray[index];

            int parentIndex = index >> 1;
            if (parentIndex > 0)
            {
                if (mSortMode == BinaryHeapSortMode.kMin)
                {
                    if (element.SortScore < mArray[parentIndex].SortScore)
                    {
                        ProcolateUp(index);
                    }
                    else
                    {
                        ProcolateDown(index);
                    }
                }
                else
                {
                    if (element.SortScore > mArray[parentIndex].SortScore)
                    {
                        ProcolateUp(index);
                    }
                    else
                    {
                        ProcolateDown(index);
                    }
                }
            }
            else
            {
                ProcolateDown(index);
            }
        }

        private void SortAsCurrentMode()
        {
            int startChild = mLastChildIndex >> 1;
            for (int i = startChild; i > 0; --i)
            {
                ProcolateDown(i);
            }
        }

        #endregion

        #region ָ��λ��ɾ��

        public void RemoveAt(int index)
        {
            if (index > mLastChildIndex || index < 1)
            {
                return;
            }

            if (index == mLastChildIndex)
            {
                --mLastChildIndex;
                mArray[index] = default(T);
                return;
            }

            mArray[index] = mArray[mLastChildIndex--];
            mArray[index].HeapIndex = index;
            RebuildAtIndex(index);
        }

        #endregion

        #region ��������

        //��������ʹ�С����֮��û���κι�ϵ
        public T GetElement(int index)
        {
            if (index > mLastChildIndex)
            {
                return default(T);
            }
            return mArray[index];
        }

        #endregion

        #region �ж�����

        public bool HasValue()
        {
            return mLastChildIndex > 0;
        }

        #endregion

        #region �ڲ�����

        protected void ResizeArray(int newSize = -1)
        {
            if (newSize < 0)
            {
                newSize = System.Math.Max(mArray.Length + 4, (int)System.Math.Round(mArray.Length * mGrowthFactor));
            }

            if (newSize > 1 << 30)
            {
                throw new System.Exception(
                    "Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. " +
                    "\nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
            }

            T[] tmp = new T[newSize];
            for (int i = 0; i < mArray.Length; i++)
            {
                tmp[i] = mArray[i];
            }

            mArray = tmp;
        }

        //�ϸ�:��Ѩ˼��
        protected void ProcolateUp(int index)
        {
            var element = mArray[index];
            if (element == null)
            {
                return;
            }

            float sortScore = element.SortScore;

            int parentIndex = index >> 1;

            if (mSortMode == BinaryHeapSortMode.kMin)
            {
                while (parentIndex >= 1 && sortScore < mArray[parentIndex].SortScore)
                {
                    mArray[index] = mArray[parentIndex];
                    mArray[index].HeapIndex = index;
                    index = parentIndex;
                    parentIndex = index >> 1;
                }
            }
            else
            {
                while (parentIndex >= 1 && sortScore > mArray[parentIndex].SortScore)
                {
                    mArray[index] = mArray[parentIndex];
                    mArray[index].HeapIndex = index;
                    index = parentIndex;
                    parentIndex = index >> 1;
                }
            }
            mArray[index] = element;
            mArray[index].HeapIndex = index;
        }

        protected void ProcolateDown(int index)
        {
            var element = mArray[index];
            if (element == null)
            {
                return;
            }

            int childIndex = index << 1;

            if (mSortMode == BinaryHeapSortMode.kMin)
            {
                while (childIndex <= mLastChildIndex)
                {
                    if (childIndex != mLastChildIndex)
                    {
                        if (mArray[childIndex + 1].SortScore < mArray[childIndex].SortScore)
                        {
                            childIndex = childIndex + 1;
                        }
                    }

                    if (mArray[childIndex].SortScore < element.SortScore)
                    {
                        mArray[index] = mArray[childIndex];
                        mArray[index].HeapIndex = index;
                    }
                    else
                    {
                        break;
                    }

                    index = childIndex;
                    childIndex = index << 1;
                }
            }
            else
            {
                while (childIndex <= mLastChildIndex)
                {
                    if (childIndex != mLastChildIndex)
                    {
                        if (mArray[childIndex + 1].SortScore > mArray[childIndex].SortScore)
                        {
                            childIndex = childIndex + 1;
                        }
                    }

                    if (mArray[childIndex].SortScore > element.SortScore)
                    {
                        mArray[index] = mArray[childIndex];
                        mArray[index].HeapIndex = index;
                    }
                    else
                    {
                        break;
                    }

                    index = childIndex;
                    childIndex = index << 1;
                }
            }

            mArray[index] = element;
            mArray[index].HeapIndex = index;
        }
        #endregion
        #endregion
    }
}