using UnityEngine;
using SceneReferencer;

[CreateAssetMenu(menuName = "Scriptables/Scene Collections", fileName = "Scene Collection", order = 4)]
public class SceneCollection : ScriptableObject
{
    public string Title;

    public SceneReference[] Collection;
}
