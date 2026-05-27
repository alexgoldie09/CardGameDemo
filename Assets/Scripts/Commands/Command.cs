using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Command
{
    /// <summary>
    /// The Command pattern is used to queue up actions that need to be executed in a specific order, with specific timing.
    /// </summary>
    public static Queue<Command> CommandQueue = new Queue<Command>();
    
    /// <summary>
    /// This bool is used to prevent multiple commands from being executed at the same time.
    /// It is set to true when a command is being executed, and set to false when the command is complete.
    /// When a command is added to the queue, if this bool is false, the command will be executed immediately.
    /// If this bool is true, the command will be added to the queue and will be executed when the current command is complete.
    /// </summary>
    public static bool playingQueue = false;

    /// <summary>
    /// This method is called to add a command to the queue.
    /// It will check if the queue is currently being played, and if not, it will start playing the queue immediately.
    /// </summary>
    public virtual void AddToQueue()
    {
        CommandQueue.Enqueue(this);
        if (!playingQueue)
            PlayFirstCommandFromQueue();
    }

    public virtual void StartCommandExecution()
    {
        // list of everything that we have to do with this command (draw a card, play a card, play spell effect, etc...)
        // there are 2 options of timing : 
        // 1) use tween sequences and call CommandExecutionComplete in OnComplete()
        // 2) use coroutines (IEnumerator) and WaitFor... to introduce delays, call CommandExecutionComplete() in the end of coroutine
    }

    /// <summary>
    /// This method is called when a command has finished executing.
    /// </summary>
    public static void CommandExecutionComplete()
    {
        if (CommandQueue.Count > 0)
            PlayFirstCommandFromQueue();
        else
            playingQueue = false;
        if (TurnManager.Instance.WhoseTurn != null)
            TurnManager.Instance.WhoseTurn.HighlightPlayableCards();
    }

    /// <summary>
    /// This method is called to start executing the first command in the queue.
    /// </summary>
    public static void PlayFirstCommandFromQueue()
    {
        playingQueue = true;
        CommandQueue.Dequeue().StartCommandExecution();
    }

    /// <summary>
    /// This method checks if there is a DrawACardCommand in the queue, which indicates that a card draw is pending.
    /// </summary>
    /// <returns></returns>
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