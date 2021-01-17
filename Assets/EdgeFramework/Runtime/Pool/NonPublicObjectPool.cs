/****************************************************
	ÎÄ¼þ£ºNonPublicObjectPool.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/14 17:14   	
	Features£º
*****************************************************/

using System;

namespace EdgeFramework
{
 /// <summary>
	/// Object pool 4 class who no public constructor
	/// such as SingletonClass.QEventSystem
	/// </summary>
	public class NonPublicObjectPool<T> :Pool<T>,ISingleton where T : class,IPoolable
	{
		#region Singleton
		public void OnSingletonInit(){}
		
		public static NonPublicObjectPool<T> Instance
		{
			get { return SingletonProperty<NonPublicObjectPool<T>>.Instance; }
		}

		protected NonPublicObjectPool()
		{
			mFactory = new NonPublicObjectFactory<T>();
		}
		
		public void Dispose()
		{
			SingletonProperty<NonPublicObjectPool<T>>.Dispose();
		}
		#endregion

		/// <summary>
		/// Init the specified maxCount and initCount.
		/// </summary>
		/// <param name="maxCount">Max Cache count.</param>
		/// <param name="initCount">Init Cache count.</param>
		public void Init(int maxCount, int initCount)
		{
			if (maxCount > 0)
			{
				initCount = Math.Min(maxCount, initCount);
			}

			if (CurCount >= initCount) return;
			
			for (var i = CurCount; i < initCount; ++i)
			{
				Recycle(mFactory.Create());
			}
		}

		/// <summary>
		/// Gets or sets the max cache count.
		/// </summary>
		/// <value>The max cache count.</value>
		public int MaxCacheCount
		{
			get { return mMaxCount; }
			set
			{
				mMaxCount = value;

				if (mCacheStack == null) return;
				if (mMaxCount <= 0) return;
				if (mMaxCount >= mCacheStack.Count) return;
				var removeCount = mMaxCount - mCacheStack.Count;
				while (removeCount > 0)
				{
					mCacheStack.Pop();
					--removeCount;
				}
			}
		}

		/// <summary>
		/// Allocate T instance.
		/// </summary>
		public override T Allocate()
		{
			var result = base.Allocate();
			result.IsRecycled = false;
			return result;
		}

		/// <summary>
		/// Recycle the T instance
		/// </summary>
		/// <param name="t">T.</param>
		public override bool Recycle(T t)
		{
			if (t == null || t.IsRecycled)
			{
				return false;
			}

			if (mMaxCount > 0)
			{
				if (mCacheStack.Count >= mMaxCount)
				{
					t.OnRecycled();
					return false;
				}
			}

			t.IsRecycled = true;
			t.OnRecycled();
			mCacheStack.Push(t);

			return true;
		}
	}
}