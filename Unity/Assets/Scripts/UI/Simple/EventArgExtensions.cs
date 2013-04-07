//#define ENFORCE_THREAD_SAFETY
using System;
#if ENFORCE_THREAD_SAFETY
using System.Threading;
#endif

internal static class EventArgExtensions
{
	public static void Raise<TEventArgs>(this TEventArgs e, Object sender, ref EventHandler<TEventArgs> eventDelegate)
		where TEventArgs : EventArgs
	{
		// Copy a reference to the delegate field into a temporary field for thread safety
#if ENFORCE_THREAD_SAFETY
		EventHandler<TEventArgs> temp =
			Interlocked.CompareExchange(ref eventDelegate, null, null);
#else
		EventHandler<TEventArgs> temp = eventDelegate;
#endif

		// If any methods registered with our event, notify them
		if (temp != null)
		{
			temp(sender, e);
		}
	}
}

