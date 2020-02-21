using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NetworkMaker
{
    /// <summary>
    /// This class inherits "NetworkDiscovery" of UNET. 
    /// This class is for the clients to discover whatevery server there is
    /// Hope it doesnt fail
    /// -made by GELO
    /// </summary>
    public class NMClientDiscovery : NetworkDiscovery
    {
        #region DELEGATES AND EVENTS
        public delegate void OnRecievedABroadcastFromServerEvent(string hostIP, int hostPort);

        public event OnRecievedABroadcastFromServerEvent OnRecievedABroadcastFromServer;
        #endregion

        Dictionary<NMLanInfoConnection, float> lanInfoDictionary = new Dictionary<NMLanInfoConnection, float>();
        float timeout = 5f;
        private void Start()
        {
            //StartCoroutine(CleanupBroadcastedLANAddress());
            base.Initialize();
        }

        public void InitializeClientDiscovery()
        {
            base.StartAsClient();
        }

        IEnumerator CleanupBroadcastedLANAddress()
        {
            while (true)
            {
                bool hasChanged = false;
                var keys = lanInfoDictionary.Keys;

                foreach (var key in keys)
                {
                    //cleanup lan if the recieved lan info is past
                    //the timeout;
                    if (lanInfoDictionary[key] <= Time.time)
                    {
                        lanInfoDictionary.Remove(key);
                        hasChanged = true;
                    }
                }

                yield return new WaitForSeconds(timeout);
            }
        }
        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            base.OnReceivedBroadcast(fromAddress, data);
            NMLanInfoConnection info = new NMLanInfoConnection(fromAddress, data);
            if (!lanInfoDictionary.ContainsKey(info))
            {
                lanInfoDictionary.Add(info, Time.time + timeout);
            }
            else
            {
                lanInfoDictionary[info] = Time.time + timeout;
            }
            RecievedABroadcast(info.GetIPAddress, info.GetPort);
        }
        void RecievedABroadcast(string ip, int port)
        {
            OnRecievedABroadcastFromServer?.Invoke(ip, port);
        }
    }
}