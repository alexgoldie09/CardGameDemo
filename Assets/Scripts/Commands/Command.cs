using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Command.cs - make it abstract so no silent empty execution is possible
public abstract class Command
{
    public static Queue<Command> CommandQueue = new Queue<Command>();
    public static bool playingQueue = false;
    public static bool IsQueueIdle => !playingQueue && CommandQueue.Count == 0;

    public virtual void AddToQueue()
    {
        CommandQueue.Enqueue(this);
        if (!playingQueue)
            PlayFirstCommandFromQueue();
    }

    // Change from virtual to abstract - forces all subclasses to implement it
    // and means no silent empty base method can be called accidentally
    public abstract void StartCommandExecution();

    public static void CommandExecutionComplete()
    {
        //Debug.Log($"[Command] ExecutionComplete - queue count:{CommandQueue.Count}");
        if (CommandQueue.Count > 0)
            PlayFirstCommandFromQueue();
        else
            playingQueue = false;
        if (TurnManager.Instance.WhoseTurn != null)
            TurnManager.Instance.WhoseTurn.HighlightPlayableCards();
    }

    public static void PlayFirstCommandFromQueue()
    {
        playingQueue = true;
        var next = CommandQueue.Peek();
        //Debug.Log($"[Command] Playing next command: {next.GetType().Name}");
        
        try
        {
            CommandQueue.Dequeue().StartCommandExecution();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Command] Exception in StartCommandExecution for {next.GetType().Name}: {e}");
            // Don't leave the queue stuck - advance it
            CommandExecutionComplete();
        }
    }

    public static bool CardDrawPending()
    {
        foreach (var c in CommandQueue)
        {
            if (c is DrawACardCommand)
                return true;
        }
        return false;
    }
}