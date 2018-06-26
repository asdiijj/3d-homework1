using UnityEngine;
using System.Collections;

public enum ActorState { ENTER, DEAD, GET }

public interface Subject
{
    void Notify(ActorState state, int i, GameObject obj);
    void Add(Observer observer);
    void Delete(Observer observer);
}

public interface Observer
{
    void Notified(ActorState state, int i, GameObject obj);
}

public class Publisher : Subject
{
    private delegate void ActionUpdate(ActorState state, int i, GameObject obj);
    private ActionUpdate updateList;

    private static Subject _instance;
    public static Subject GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Publisher();
        }
        return _instance;
    }

    public void Notify(ActorState state, int i, GameObject obj)
    {
        if (updateList != null)
        {
            updateList(state, i, obj);
        }
    }
    public void Add(Observer observer)
    {
        updateList += observer.Notified;
    }
    public void Delete(Observer observer)
    {
        updateList -= observer.Notified;
    }
}