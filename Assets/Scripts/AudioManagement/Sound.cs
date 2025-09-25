using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;


#if UNITY_EDITOR
using UnityEditor;
#endif

public struct Sound
{
    public AudioClip clip;
    public float volume, pitch;

    [HideInInspector] public bool foldout;

    public SoundData ToSoundData(ClipDatabase db)
    {
        return new()
        {
            clipIndex = db.GetIndex(clip),
            volume = volume,
            pitch = pitch
        };
    }
}
public struct SoundData : INetworkSerializable
{
    public int clipIndex;
    public float volume, pitch;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clipIndex);
        serializer.SerializeValue(ref volume);
        serializer.SerializeValue(ref pitch);
    }

    public Sound ToSound(ClipDatabase db)
    {
        return new()
        {
            clip = db.GetClip(clipIndex),
            volume = volume,
            pitch = pitch
        };
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Sound))]
public class Sound_Drawer : PropertyDrawer
{
    AudioSource testSource;
    const float rightBoxWidth = 50.0f;
    const float foldoutHeight = 20.0f;
    const float contentHeight = 60.0f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AudioSource tmp;
        SerializedProperty foldout = property.FindPropertyRelative("foldout");
        foldout.boolValue = EditorGUI.Foldout(new Rect(position) { height = foldoutHeight }, foldout.boolValue, label);
        if (foldout.boolValue)
        {
            position.y += foldoutHeight;
            SerializedProperty clipProperty = property.FindPropertyRelative("clip");
            SerializedProperty volumeProperty = property.FindPropertyRelative("volume");
            SerializedProperty pitchProperty = property.FindPropertyRelative("pitch");

            Rect leftBox = position, rightBox = position;
            leftBox.width = position.width - rightBoxWidth;
            rightBox.x = position.x + position.width - rightBoxWidth;
            rightBox.width = rightBoxWidth;

            EditorGUI.PropertyField(new Rect(leftBox) { height = 20 }, clipProperty);
            volumeProperty.floatValue = EditorGUI.Slider(new Rect(leftBox) { y = leftBox.y + 20, height = 20 }, new GUIContent() { text = "Volume" }, volumeProperty.floatValue, 0, 1);
            pitchProperty.floatValue = EditorGUI.Slider(new Rect(leftBox) { y = leftBox.y + 40, height = 20 }, new GUIContent() { text = "Pitch" }, pitchProperty.floatValue, 0, 2);

            if (GUI.Button(new Rect(rightBox) { height = 40 }, "Play"))
            {
                if (testSource == null)
                {
                    testSource = new GameObject() { hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild }.AddComponent<AudioSource>();
                    testSource.playOnAwake = false;
                }
                testSource.clip = clipProperty.objectReferenceValue as AudioClip;
                testSource.volume = volumeProperty.floatValue;
                testSource.pitch = pitchProperty.floatValue;
                testSource.Play();
            }
            if (GUI.Button(new Rect(rightBox) { y = rightBox.y + 40, height = 40 }, "Stop"))
            {
                if (testSource != null) testSource.Pause();
            }
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = foldoutHeight;
        SerializedProperty foldout = property.FindPropertyRelative("foldout");
        if (foldout.boolValue)
        {
            height += contentHeight;
        }
        return height;
    }
}
#endif