﻿using System;
using System.Collections.Generic;

namespace MediaBrowser.Model.Dlna
{
    public class MediaFormatProfileResolver
    {
        public IEnumerable<MediaFormatProfile> ResolveVideoFormat(string container, string videoCodec, string audioCodec, int? width, int? height, TransportStreamTimestamp timestampType)
        {
            if (string.Equals(container, "asf", StringComparison.OrdinalIgnoreCase))
            {
                var val = ResolveVideoASFFormat(videoCodec, audioCodec, width, height);
                return val.HasValue ? new List<MediaFormatProfile> { val.Value } : new List<MediaFormatProfile>();
            }

            if (string.Equals(container, "mp4", StringComparison.OrdinalIgnoreCase))
            {
                var val = ResolveVideoMP4Format(videoCodec, audioCodec, width, height);
                return val.HasValue ? new List<MediaFormatProfile> { val.Value } : new List<MediaFormatProfile>();
            }

            if (string.Equals(container, "avi", StringComparison.OrdinalIgnoreCase))
                return new[] { MediaFormatProfile.AVI };

            if (string.Equals(container, "mkv", StringComparison.OrdinalIgnoreCase))
                return new[] { MediaFormatProfile.MATROSKA };

            if (string.Equals(container, "mpeg2ps", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(container, "ts", StringComparison.OrdinalIgnoreCase))

                return new[] { MediaFormatProfile.MPEG_PS_NTSC, MediaFormatProfile.MPEG_PS_PAL };

            if (string.Equals(container, "mpeg1video", StringComparison.OrdinalIgnoreCase))
                return new[] { MediaFormatProfile.MPEG1 };

            if (string.Equals(container, "mpeg2ts", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(container, "mpegts", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(container, "m2ts", StringComparison.OrdinalIgnoreCase))
            {

                return ResolveVideoMPEG2TSFormat(videoCodec, audioCodec, width, height, timestampType);
            }

            if (string.Equals(container, "flv", StringComparison.OrdinalIgnoreCase))
                return new[] { MediaFormatProfile.FLV };

            if (string.Equals(container, "wtv", StringComparison.OrdinalIgnoreCase))
                return new[] { MediaFormatProfile.WTV };

            if (string.Equals(container, "3gp", StringComparison.OrdinalIgnoreCase))
            {
                var val = ResolveVideo3GPFormat(videoCodec, audioCodec);
                return val.HasValue ? new List<MediaFormatProfile> { val.Value } : new List<MediaFormatProfile>();
            }

            if (string.Equals(container, "ogv", StringComparison.OrdinalIgnoreCase) || string.Equals(container, "ogg", StringComparison.OrdinalIgnoreCase))
                return new[] { MediaFormatProfile.OGV };

            return new List<MediaFormatProfile>();
        }

        private IEnumerable<MediaFormatProfile> ResolveVideoMPEG2TSFormat(string videoCodec, string audioCodec, int? width, int? height, TransportStreamTimestamp timestampType)
        {
            var suffix = "";

            switch (timestampType)
            {
                case TransportStreamTimestamp.NONE:
                    suffix = "_ISO";
                    break;
                case TransportStreamTimestamp.VALID:
                    suffix = "_T";
                    break;
            }

            var resolution = "S";
            if ((width.HasValue && width.Value > 720) || (height.HasValue && height.Value > 576))
            {
                resolution = "H";
            }

            if (string.Equals(videoCodec, "mpeg2video", StringComparison.OrdinalIgnoreCase))
            {
                var list = new List<MediaFormatProfile>();

                list.Add(ValueOf("MPEG_TS_SD_NA" + suffix));
                list.Add(ValueOf("MPEG_TS_SD_EU" + suffix));
                list.Add(ValueOf("MPEG_TS_SD_KO" + suffix));

                if ((timestampType == TransportStreamTimestamp.VALID) && string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(MediaFormatProfile.MPEG_TS_JP_T);
                }
                return list;
            }
            if (string.Equals(videoCodec, "h264", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(audioCodec, "lpcm", StringComparison.OrdinalIgnoreCase))
                    return new[] { MediaFormatProfile.AVC_TS_HD_50_LPCM_T };

                if (string.Equals(audioCodec, "dts", StringComparison.OrdinalIgnoreCase))
                {
                    if (timestampType == TransportStreamTimestamp.NONE)
                    {
                        return new[] { MediaFormatProfile.AVC_TS_HD_DTS_ISO };
                    }
                    return new[] { MediaFormatProfile.AVC_TS_HD_DTS_T };
                }

                if (string.Equals(audioCodec, "mp3", StringComparison.OrdinalIgnoreCase))
                {
                    if (timestampType == TransportStreamTimestamp.NONE)
                    {
                        return new[] { ValueOf(string.Format("AVC_TS_HP_{0}D_MPEG1_L2_ISO", resolution)) };
                    }

                    return new[] { ValueOf(string.Format("AVC_TS_HP_{0}D_MPEG1_L2_T", resolution)) };
                }

                if (string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                    return new[] { ValueOf(string.Format("AVC_TS_MP_{0}D_AAC_MULT5{1}", resolution, suffix)) };

                if (string.Equals(audioCodec, "mp3", StringComparison.OrdinalIgnoreCase))
                    return new[] { ValueOf(string.Format("AVC_TS_MP_{0}D_MPEG1_L3{1}", resolution, suffix)) };

                if (string.IsNullOrEmpty(audioCodec) ||
                    string.Equals(audioCodec, "ac3", StringComparison.OrdinalIgnoreCase))
                    return new[] { ValueOf(string.Format("AVC_TS_MP_{0}D_AC3{1}", resolution, suffix)) };
            }
            else if (string.Equals(videoCodec, "vc1", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "ac3", StringComparison.OrdinalIgnoreCase))
                {
                    if ((width.HasValue && width.Value > 720) || (height.HasValue && height.Value > 576))
                    {
                        return new[] { MediaFormatProfile.VC1_TS_AP_L2_AC3_ISO };
                    }
                    return new[] { MediaFormatProfile.VC1_TS_AP_L1_AC3_ISO };
                }
                if (string.Equals(audioCodec, "dts", StringComparison.OrdinalIgnoreCase))
                {
                    suffix = string.Equals(suffix, "_ISO") ? suffix : "_T";

                    return new[] { ValueOf(string.Format("VC1_TS_HD_DTS{0}", suffix)) };
                }

            }
            else if (string.Equals(videoCodec, "mpeg4", StringComparison.OrdinalIgnoreCase) || string.Equals(videoCodec, "msmpeg4", StringComparison.OrdinalIgnoreCase))
            {
                //  if (audioCodec == AudioCodec.AAC)
                //    return Collections.singletonList(MediaFormatProfile.valueOf(String.format("MPEG4_P2_TS_ASP_AAC%s", cast(Object[])[ suffix ])));
                //  if (audioCodec == AudioCodec.MP3)
                //    return Collections.singletonList(MediaFormatProfile.valueOf(String.format("MPEG4_P2_TS_ASP_MPEG1_L3%s", cast(Object[])[ suffix ])));
                //  if (audioCodec == AudioCodec.MP2)
                //    return Collections.singletonList(MediaFormatProfile.valueOf(String.format("MPEG4_P2_TS_ASP_MPEG2_L2%s", cast(Object[])[ suffix ])));
                //  if ((audioCodec is null) || (audioCodec == AudioCodec.AC3)) {
                //    return Collections.singletonList(MediaFormatProfile.valueOf(String.format("MPEG4_P2_TS_ASP_AC3%s", cast(Object[])[ suffix ])));
            }

            return new List<MediaFormatProfile>();
        }

        private MediaFormatProfile ValueOf(string value)
        {
            return (MediaFormatProfile)Enum.Parse(typeof(MediaFormatProfile), value, true);
        }

        private MediaFormatProfile? ResolveVideoMP4Format(string videoCodec, string audioCodec, int? width, int? height)
        {
            if (string.Equals(videoCodec, "h264", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(audioCodec, "lpcm", StringComparison.OrdinalIgnoreCase))
                    return MediaFormatProfile.AVC_MP4_LPCM;
                if (string.IsNullOrEmpty(audioCodec) ||
                    string.Equals(audioCodec, "ac3", StringComparison.OrdinalIgnoreCase))
                {
                    return MediaFormatProfile.AVC_MP4_MP_SD_AC3;
                }
                if (string.Equals(audioCodec, "mp3", StringComparison.OrdinalIgnoreCase))
                {
                    return MediaFormatProfile.AVC_MP4_MP_SD_MPEG1_L3;
                }
                if (width.HasValue && height.HasValue)
                {
                    if ((width.Value <= 720) && (height.Value <= 576))
                    {
                        if (string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                            return MediaFormatProfile.AVC_MP4_MP_SD_AAC_MULT5;
                    }
                    else if ((width.Value <= 1280) && (height.Value <= 720))
                    {
                        if (string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                            return MediaFormatProfile.AVC_MP4_MP_HD_720p_AAC;
                    }
                    else if ((width.Value <= 1920) && (height.Value <= 1080))
                    {
                        if (string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                        {
                            return MediaFormatProfile.AVC_MP4_MP_HD_1080i_AAC;
                        }
                    }
                }
            }
            else if (string.Equals(videoCodec, "mpeg4", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(videoCodec, "msmpeg4", StringComparison.OrdinalIgnoreCase))
            {
                if (width.HasValue && height.HasValue && width.Value <= 720 && height.Value <= 576)
                {
                    if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                        return MediaFormatProfile.MPEG4_P2_MP4_ASP_AAC;
                    if (string.Equals(audioCodec, "ac3", StringComparison.OrdinalIgnoreCase) || string.Equals(audioCodec, "mp3", StringComparison.OrdinalIgnoreCase))
                    {
                        return MediaFormatProfile.MPEG4_P2_MP4_NDSD;
                    }
                }
                else if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                {
                    return MediaFormatProfile.MPEG4_P2_MP4_SP_L6_AAC;
                }
            }
            else if (string.Equals(videoCodec, "h263", StringComparison.OrdinalIgnoreCase) && string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
            {
                return MediaFormatProfile.MPEG4_H263_MP4_P0_L10_AAC;
            }

            return null;
        }

        private MediaFormatProfile? ResolveVideo3GPFormat(string videoCodec, string audioCodec)
        {
            if (string.Equals(videoCodec, "h264", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "aac", StringComparison.OrdinalIgnoreCase))
                    return MediaFormatProfile.AVC_3GPP_BL_QCIF15_AAC;
            }
            else if (string.Equals(videoCodec, "mpeg4", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(videoCodec, "msmpeg4", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "wma", StringComparison.OrdinalIgnoreCase))
                    return MediaFormatProfile.MPEG4_P2_3GPP_SP_L0B_AAC;
                if (string.Equals(audioCodec, "amrnb", StringComparison.OrdinalIgnoreCase))
                    return MediaFormatProfile.MPEG4_P2_3GPP_SP_L0B_AMR;
            }
            else if (string.Equals(videoCodec, "h263", StringComparison.OrdinalIgnoreCase) && string.Equals(audioCodec, "amrnb", StringComparison.OrdinalIgnoreCase))
            {
                return MediaFormatProfile.MPEG4_H263_3GPP_P0_L10_AMR;
            }

            return null;
        }

        private MediaFormatProfile? ResolveVideoASFFormat(string videoCodec, string audioCodec, int? width, int? height)
        {
            if (string.Equals(videoCodec, "wmv", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "wma", StringComparison.OrdinalIgnoreCase) || string.Equals(videoCodec, "wmapro", StringComparison.OrdinalIgnoreCase)))
            {

                if (width.HasValue && height.HasValue)
                {
                    if ((width.Value <= 720) && (height.Value <= 576))
                    {
                        if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "wma", StringComparison.OrdinalIgnoreCase))
                        {
                            return MediaFormatProfile.WMVMED_FULL;
                        }
                        return MediaFormatProfile.WMVMED_PRO;
                    }
                }

                if (string.IsNullOrEmpty(audioCodec) || string.Equals(audioCodec, "wma", StringComparison.OrdinalIgnoreCase))
                {
                    return MediaFormatProfile.WMVHIGH_FULL;
                }
                return MediaFormatProfile.WMVHIGH_PRO;
            }

            if (string.Equals(videoCodec, "vc1", StringComparison.OrdinalIgnoreCase))
            {
                if (width.HasValue && height.HasValue)
                {
                    if ((width.Value <= 720) && (height.Value <= 576))
                        return MediaFormatProfile.VC1_ASF_AP_L1_WMA;
                    if ((width.Value <= 1280) && (height.Value <= 720))
                        return MediaFormatProfile.VC1_ASF_AP_L2_WMA;
                    if ((width.Value <= 1920) && (height.Value <= 1080))
                        return MediaFormatProfile.VC1_ASF_AP_L3_WMA;
                }
            }
            else if (string.Equals(videoCodec, "mpeg2video", StringComparison.OrdinalIgnoreCase))
            {
                return MediaFormatProfile.DVR_MS;
            }

            return null;
        }

        public MediaFormatProfile? ResolveAudioFormat(string container, int? bitrate, int? frequency, int? channels)
        {
            if (string.Equals(container, "asf", StringComparison.OrdinalIgnoreCase))
                return ResolveAudioASFFormat(bitrate);

            if (string.Equals(container, "mp3", StringComparison.OrdinalIgnoreCase))
                return MediaFormatProfile.MP3;

            if (string.Equals(container, "lpcm", StringComparison.OrdinalIgnoreCase))
                return ResolveAudioLPCMFormat(frequency, channels);

            if (string.Equals(container, "mp4", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(container, "aac", StringComparison.OrdinalIgnoreCase))
                return ResolveAudioMP4Format(bitrate);

            if (string.Equals(container, "adts", StringComparison.OrdinalIgnoreCase))
                return ResolveAudioADTSFormat(bitrate);

            if (string.Equals(container, "flac", StringComparison.OrdinalIgnoreCase))
                return MediaFormatProfile.FLAC;

            if (string.Equals(container, "oga", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(container, "ogg", StringComparison.OrdinalIgnoreCase))
                return MediaFormatProfile.OGG;

            return null;
        }

        private MediaFormatProfile ResolveAudioASFFormat(int? bitrate)
        {
            if (bitrate.HasValue && bitrate.Value <= 193)
            {
                return MediaFormatProfile.WMA_BASE;
            }
            return MediaFormatProfile.WMA_FULL;
        }

        private MediaFormatProfile? ResolveAudioLPCMFormat(int? frequency, int? channels)
        {
            if (frequency.HasValue && channels.HasValue)
            {
                if (frequency.Value == 44100 && channels.Value == 1)
                {
                    return MediaFormatProfile.LPCM16_44_MONO;
                }
                if (frequency.Value == 44100 && channels.Value == 2)
                {
                    return MediaFormatProfile.LPCM16_44_STEREO;
                }
                if (frequency.Value == 48000 && channels.Value == 1)
                {
                    return MediaFormatProfile.LPCM16_48_MONO;
                }
                if (frequency.Value == 48000 && channels.Value == 1)
                {
                    return MediaFormatProfile.LPCM16_48_STEREO;
                }

                return null;
            }

            return MediaFormatProfile.LPCM16_48_STEREO;
        }

        private MediaFormatProfile ResolveAudioMP4Format(int? bitrate)
        {
            if (bitrate.HasValue && bitrate.Value <= 320)
            {
                return MediaFormatProfile.AAC_ISO_320;
            }
            return MediaFormatProfile.AAC_ISO;
        }

        private MediaFormatProfile ResolveAudioADTSFormat(int? bitrate)
        {
            if (bitrate.HasValue && bitrate.Value <= 320)
            {
                return MediaFormatProfile.AAC_ADTS_320;
            }
            return MediaFormatProfile.AAC_ADTS;
        }

        public MediaFormatProfile? ResolveImageFormat(string container, int? width, int? height)
        {
            if (string.Equals(container, "jpeg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(container, "jpg", StringComparison.OrdinalIgnoreCase))
                return ResolveImageJPGFormat(width, height);

            if (string.Equals(container, "png", StringComparison.OrdinalIgnoreCase))
                return MediaFormatProfile.PNG_LRG;

            if (string.Equals(container, "gif", StringComparison.OrdinalIgnoreCase))
                return MediaFormatProfile.GIF_LRG;

            if (string.Equals(container, "raw", StringComparison.OrdinalIgnoreCase))
                return MediaFormatProfile.RAW;

            return null;
        }

        private MediaFormatProfile ResolveImageJPGFormat(int? width, int? height)
        {
            if (width.HasValue && height.HasValue)
            {
                if ((width.Value <= 640) && (height.Value <= 480))
                    return MediaFormatProfile.JPEG_SM;

                if ((width.Value <= 1024) && (height.Value <= 768))
                {
                    return MediaFormatProfile.JPEG_MED;
                }
            }

            return MediaFormatProfile.JPEG_LRG;
        }
    }
}
