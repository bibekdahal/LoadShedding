using System;

public class AsyncSemaphore
{
    public AsyncSemaphore(int initialCount);
    public Task WaitAsync();
    public void Release();

    private readonly static Task s_completed = Task.FromResult(true);
    private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
    private int m_currentCount; 
}
