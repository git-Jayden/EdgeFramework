using UnityEngine;
using System.Collections;

public interface IState
{
    void onEnter();
    void onUpdate(float step);
    void onExit();



    string getName();

}