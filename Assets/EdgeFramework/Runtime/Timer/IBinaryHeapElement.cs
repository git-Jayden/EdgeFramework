/****************************************************
	�ļ���IBinaryHeapElement.cs
	Author��JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date��2021/03/11 11:27   	
	Features��
*****************************************************/
namespace EdgeFramework
{
    public interface IBinaryHeapElement
    {
        float SortScore { get; }

        int HeapIndex { set; }

        void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement;
    }

}