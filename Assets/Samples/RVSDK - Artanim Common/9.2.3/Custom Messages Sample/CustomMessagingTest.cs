using MessagePack;
using RvSdk.Controller;
using RvSdk.Module.Messages;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace RvSdk.Samples
{
    public class CustomMessagingTest : MonoBehaviour
    {
        private void Start()
        {
            //delete this warning
            Debug.LogWarning("Please uncomment the following code to use the Custom Message Sample");
        }
        //uncomment everything under this line
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        //private void OnEnable()
        //{
        //    if (TextReliable) TextReliable.text = "No update";
        //    if (TextStreaming) TextStreaming.text = "No update";

        //    //Subscribe to custom messages
        //    NetworkController.SubscribeUnityMessage<CustomReliableMessage>(OnCustomReliableMessage);
        //    NetworkController.SubscribeUnityMessage<CustomStreamingMessage>(OnCustomStreamingMessage);

        //    //Start sender routines on the server only
        //    if (NetworkController.IsServer)
        //    {
        //        StartCoroutine(ServerReliableSenderRoutine());
        //        StartCoroutine(ServerStreamingRoutine());
        //    }
        //}

        //private void OnDisable()
        //{
        //    StopAllCoroutines();

        //    //Unsubscribe listeners
        //    NetworkController.UnsubscribeUnityMessage<CustomReliableMessage>(OnCustomReliableMessage);
        //    NetworkController.UnsubscribeUnityMessage<CustomStreamingMessage>(OnCustomStreamingMessage);
        //}

        //#region Reliable

        //[SerializeField] TextMeshProUGUI TextReliable;

        ///// <summary>
        ///// Custom reliable message using property names as keys
        ///// </summary>
        //[MessagePackObject(true)]
        //public class CustomReliableMessage : UnityReliableMessagePack
        //{
        //    public string TimeString { get; set; }
        //}

        //private IEnumerator ServerReliableSenderRoutine()
        //{
        //    while (true)
        //    {
        //        NetworkController.SendMessageToSession(new CustomReliableMessage { TimeString = DateTime.Now.ToLongTimeString() });
        //        yield return new WaitForSecondsRealtime(0.5f);
        //    }
        //}

        //private void OnCustomReliableMessage(CustomReliableMessage message)
        //{
        //    if (TextReliable) TextReliable.text = $"Time: {message.TimeString} <i>(Reliable)</i>";
        //}

        //#endregion

        //#region Streaming

        //[SerializeField] TextMeshProUGUI TextStreaming;

        ///// <summary>
        ///// Custom streaming message using [Key(...)] attributes
        ///// </summary>
        //public class CustomStreamingMessage : UnityStreamingMessagePack
        //{
        //    [Key(0)] public int FrameNumber;
        //    [IgnoreMember] public bool Ignored;
        //}

        //private IEnumerator ServerStreamingRoutine()
        //{
        //    while (true)
        //    {
        //        NetworkController.SendMessageToSession(new CustomStreamingMessage { FrameNumber = Time.frameCount });
        //        yield return new WaitForSecondsRealtime(0.05f);
        //    }
        //}

        //private void OnCustomStreamingMessage(CustomStreamingMessage message)
        //{
        //    if (TextStreaming) TextStreaming.text = $"Frame Number: {message.FrameNumber:0000} <i>(Streaming)</i>";
        //}

        //#endregion
    }
}
