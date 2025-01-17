﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace OpusDotNet
{
    internal static class API
    {
        // Support for iOS builds in Unity
#if __IOS__ || UNITY_IOS && !UNITY_EDITOR
        private const string LibraryPath = "__Internal";
#else
        private const string LibraryPath = "opus";
#endif
        
        // Encoder

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern SafeEncoderHandle opus_encoder_create(int Fs, int channels, int application, out int error);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void opus_encoder_destroy(IntPtr st);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_encode(SafeEncoderHandle st, IntPtr pcm, int frame_size, IntPtr data, int max_data_bytes);
        
        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_encode_float(SafeEncoderHandle st, IntPtr pcm, int frame_size, IntPtr data, int max_data_bytes);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_encoder_ctl(SafeEncoderHandle st, int request, out int value);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_encoder_ctl(SafeEncoderHandle st, int request, int value);

        // Decoder

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern SafeDecoderHandle opus_decoder_create(int Fs, int channels, out int error);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void opus_decoder_destroy(IntPtr st);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_decode(SafeDecoderHandle st, IntPtr data, int len, IntPtr pcm, int frame_size, int decode_fec);
        
        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_decode_float(SafeDecoderHandle st, byte[] data, int len, float[] pcm, int frame_size, int decode_fec);

        // Helper Methods
        
        [DllImport(LibraryPath, EntryPoint = "opus_get_version_string", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpusVersionString();

        public static string GetLibraryVersionString()
        {
            return Marshal.PtrToStringAnsi(OpusVersionString());
        }

        public static int GetSampleCount(double frameSize, int sampleRate)
        {
            // Number of samples per channel.
            return (int)(frameSize * sampleRate / 1000);
        }

        public static int GetPCMLength(int samples, int channels)
        {
            // 16-bit audio contains a sample every 2 (16 / 8) bytes, so we multiply by 2.
            return samples * channels * 2;
        }

        public static double GetFrameSize(int pcmLength, int sampleRate, int channels)
        {
            return (double)pcmLength / sampleRate / channels / 2 * 1000;
        }

        public static void ThrowIfError(int result)
        {
            if (result < 0)
            {
                throw new OpusException(result);
            }
        }
    }
}
