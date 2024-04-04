using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorControlDemo.Models
{
    public class HikDoorEnum
    {
        public enum DoorStatus
        {
            /// <summary>
            /// Invalid
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// Sleep
            /// </summary>
            Sleep,
            /// <summary>
            /// Normally open
            /// </summary>
            NormallyOpen,
            /// <summary>
            /// Normally closed
            /// </summary>
            NormallyClosed,
            /// <summary>
            /// Normal status
            /// </summary>
            Normal
        }

        public enum CardReadVerifyMode
        {
            /// <summary>
            /// Invalid
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// Sleep
            /// </summary>
            Sleep,
            /// <summary>
            /// Card and password
            /// </summary>
            CardAndPassword,
            /// <summary>
            /// Card only
            /// </summary>
            CardOnly,
            /// <summary>
            /// Card or password
            /// </summary>
            CardOrPassword,
            /// <summary>
            /// Fingerprint
            /// </summary>
            Fingerprint,
            /// <summary>
            /// Fingerprint and password
            /// </summary>
            FingerprintAndPassword,
            /// <summary>
            /// Fingerprint or card
            /// </summary>
            FingerprintOrCard,
            /// <summary>
            /// Fingerprint and card
            /// </summary>
            FingerprintAndCard,
            /// <summary>
            /// Fingerprint, card, and password (no order)
            /// </summary>
            FingerprintCardAndPassword
        }

        public enum ConfigCommand
        {
            /// <summary>
            /// Set door parameters, corresponding to NET_DVR_DOOR_CFG structure
            /// </summary>
            NET_DVR_SET_DOOR_CFG = 2109,
            /// <summary>
            /// Get door parameters, corresponding to NET_DVR_DOOR_CFG structure
            /// </summary>
            NET_DVR_GET_DOOR_CFG = 2108,
            /// <summary>
            /// Get access control system work status, corresponding to NET_DVR_ACS_WORK_STATUS structure
            /// </summary>
            NET_DVR_GET_ACS_WORK_STATUS = 2123,
            /// <summary>
            /// Get time parameters, corresponding to NET_DVR_TIME structure
            /// </summary>
            NET_DVR_GET_TIMECFG = 118,
            /// <summary>
            /// Set time parameters, corresponding to NET_DVR_TIME structure
            /// </summary>
            NET_DVR_SET_TIMECFG = 119,
            /// <summary>
            /// Get device parameters (extended), corresponding to NET_DVR_DEVICECFG_V40 structure
            /// </summary>
            NET_DVR_GET_DEVICECFG_V40 = 1100,
            /// <summary>
            /// Get card reader parameters, corresponding to NET_DVR_CARD_READER_CFG structure
            /// </summary>
            NET_DVR_GET_CARD_READER_CFG = 2140,
            /// <summary>
            /// Set card reader parameters, corresponding to NET_DVR_CARD_READER_CFG structure
            /// </summary>
            NET_DVR_SET_CARD_READER_CFG = 2141,
            // Other enum members omitted for brevity...
            NET_DVR_SET_VERIFY_WEEK_PLAN = 2125,
        }


    }
}
