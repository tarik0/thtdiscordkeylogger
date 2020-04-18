using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;

// 4krep212 Kullanıcısına Exe Icon Değiştirici programı için teşekkür ediyorum.



namespace THT_Discord_Keylogger_Generator
{
#pragma warning disable 649
    // Token: 0x02000008 RID: 8
    public class IconInjector
    {
        // Token: 0x0600001F RID: 31 RVA: 0x0011F6C8 File Offset: 0x0011DAC8
        [DebuggerNonUserCode()]
        public IconInjector()
        {
        }

        // Token: 0x06000020 RID: 32 RVA: 0x0011F6D4 File Offset: 0x0011DAD4
        public static void InjectIcon(string exeFileName, string iconFileName)
        {
            IconInjector.InjectIcon(exeFileName, iconFileName, 1U, 1U);
        }

        public static void InjectIcon(string exeFileName, byte[] array)
        {
            IconInjector.InjectIcon(exeFileName, array, 1U, 1U);
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0011F6E4 File Offset: 0x0011DAE4
        public static void InjectIcon(string exeFileName, byte[] _array, uint iconGroupID, uint iconBaseID)
        {
            IconInjector.IconFile iconFile = IconInjector.IconFile.FromArray(_array);
            IntPtr intPtr = IconInjector.NativeMethods.BeginUpdateResource(exeFileName, false);
            byte[] array = iconFile.CreateIconGroupData(iconBaseID);
            IntPtr hUpdate = intPtr;
            IntPtr intPtr2 = new IntPtr(14L);
            IntPtr type = intPtr2;
            IntPtr intPtr3 = new IntPtr(System.Convert.ToInt64(System.Convert.ToUInt64(iconGroupID)));
            IconInjector.NativeMethods.UpdateResource(hUpdate, type, intPtr3, 0, array, array.Length);
            int num = 0;
            int num2 = iconFile.ImageCount - 1;
            int num3 = num;
            while (true)
            {
                int num4 = num3;
                int num5 = num2;
                if (num4 > num5)
                    break;
                byte[] array2 = iconFile.ImageData(num3);
                IntPtr hUpdate2 = intPtr;
                intPtr3 = new IntPtr(3L);
                IntPtr type2 = intPtr3;
                intPtr2 = new IntPtr(System.Convert.ToInt64((System.Convert.ToUInt64(iconBaseID) + System.Convert.ToUInt64((System.Convert.ToInt64(num3))))));
                IconInjector.NativeMethods.UpdateResource(hUpdate2, type2, intPtr2, 0, array2, array2.Length);
                num3 += 1;
            }
            IconInjector.NativeMethods.EndUpdateResource(intPtr, false);
        }

        public static void InjectIcon(string exeFileName, string iconFileName, uint iconGroupID, uint iconBaseID)
        {
            IconInjector.IconFile iconFile = IconInjector.IconFile.FromFile(iconFileName);
            IntPtr intPtr = IconInjector.NativeMethods.BeginUpdateResource(exeFileName, false);
            byte[] array = iconFile.CreateIconGroupData(iconBaseID);
            IntPtr hUpdate = intPtr;
            IntPtr intPtr2 = new IntPtr(14L);
            IntPtr type = intPtr2;
            IntPtr intPtr3 = new IntPtr(System.Convert.ToInt64(System.Convert.ToUInt64(iconGroupID)));
            IconInjector.NativeMethods.UpdateResource(hUpdate, type, intPtr3, 0, array, array.Length);
            int num = 0;
            int num2 = iconFile.ImageCount - 1;
            int num3 = num;
            while (true)
            {
                int num4 = num3;
                int num5 = num2;
                if (num4 > num5)
                    break;
                byte[] array2 = iconFile.ImageData(num3);
                IntPtr hUpdate2 = intPtr;
                intPtr3 = new IntPtr(3L);
                IntPtr type2 = intPtr3;
                intPtr2 = new IntPtr(System.Convert.ToInt64((System.Convert.ToUInt64(iconBaseID) + System.Convert.ToUInt64((System.Convert.ToInt64(num3))))));
                IconInjector.NativeMethods.UpdateResource(hUpdate2, type2, intPtr2, 0, array2, array2.Length);
                num3 += 1;
            }
            IconInjector.NativeMethods.EndUpdateResource(intPtr, false);
        }

        // Token: 0x02000009 RID: 9
        [SuppressUnmanagedCodeSecurity()]
        private class NativeMethods
        {
            // Token: 0x06000022 RID: 34 RVA: 0x0011F788 File Offset: 0x0011DB88
            [DebuggerNonUserCode()]
            public NativeMethods()
            {
            }

            // Token: 0x06000023 RID: 35
            [System.Runtime.InteropServices.DllImport("kernel32")]
            public static extern IntPtr BeginUpdateResource(string fileName, [MarshalAs(UnmanagedType.Bool)] bool deleteExistingResources);

            // Token: 0x06000024 RID: 36
            [System.Runtime.InteropServices.DllImport("kernel32")]
            public static extern bool UpdateResource(IntPtr hUpdate, IntPtr type, IntPtr name, short language, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] data, int dataSize);

            // Token: 0x06000025 RID: 37
            [System.Runtime.InteropServices.DllImport("kernel32")]
            public static extern bool EndUpdateResource(IntPtr hUpdate, [MarshalAs(UnmanagedType.Bool)] bool discard);
        }

        // Token: 0x0200000A RID: 10
        private struct ICONDIR
        {
            // Token: 0x0400000A RID: 10
            public ushort Reserved;

            // Token: 0x0400000B RID: 11
            public ushort Type;

            // Token: 0x0400000C RID: 12
            public ushort Count;
        }

        // Token: 0x0200000B RID: 11
        private struct ICONDIRENTRY
        {
            // Token: 0x0400000D RID: 13
            public byte Width;

            // Token: 0x0400000E RID: 14
            public byte Height;

            // Token: 0x0400000F RID: 15
            public byte ColorCount;

            // Token: 0x04000010 RID: 16
            public byte Reserved;

            // Token: 0x04000011 RID: 17
            public ushort Planes;

            // Token: 0x04000012 RID: 18
            public ushort BitCount;

            // Token: 0x04000013 RID: 19
            public int BytesInRes;

            // Token: 0x04000014 RID: 20
            public int ImageOffset;
        }

        // Token: 0x0200000C RID: 12
        private struct BITMAPINFOHEADER
        {
            // Token: 0x04000015 RID: 21
            public uint Size;

            // Token: 0x04000016 RID: 22
            public int Width;

            // Token: 0x04000017 RID: 23
            public int Height;

            // Token: 0x04000018 RID: 24
            public ushort Planes;

            // Token: 0x04000019 RID: 25
            public ushort BitCount;

            // Token: 0x0400001A RID: 26
            public uint Compression;

            // Token: 0x0400001B RID: 27
            public uint SizeImage;

            // Token: 0x0400001C RID: 28
            public int XPelsPerMeter;

            // Token: 0x0400001D RID: 29
            public int YPelsPerMeter;

            // Token: 0x0400001E RID: 30
            public uint ClrUsed;

            // Token: 0x0400001F RID: 31
            public uint ClrImportant;
        }

        // Token: 0x0200000D RID: 13
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private struct GRPICONDIRENTRY
        {
            // Token: 0x04000020 RID: 32
            public byte Width;

            // Token: 0x04000021 RID: 33
            public byte Height;

            // Token: 0x04000022 RID: 34
            public byte ColorCount;

            // Token: 0x04000023 RID: 35
            public byte Reserved;

            // Token: 0x04000024 RID: 36
            public ushort Planes;

            // Token: 0x04000025 RID: 37
            public ushort BitCount;

            // Token: 0x04000026 RID: 38
            public int BytesInRes;

            // Token: 0x04000027 RID: 39
            public ushort ID;
        }

        // Token: 0x0200000E RID: 14
        private class IconFile
        {
            // Token: 0x17000008 RID: 8
            // (get) Token: 0x06000026 RID: 38 RVA: 0x0011F794 File Offset: 0x0011DB94
            public int ImageCount
            {
                get
                {
                    return System.Convert.ToInt32(this.iconDir.Count);
                }
            }

            // Token: 0x17000009 RID: 9
            // (get) Token: 0x06000027 RID: 39 RVA: 0x0011F7B4 File Offset: 0x0011DBB4
            public byte[] ImageData(int index)
            {
                return this.iconImage[index];
            }

            // Token: 0x06000028 RID: 40 RVA: 0x0011F7D0 File Offset: 0x0011DBD0
            private IconFile()
            {
                this.iconDir = default(ICONDIR);
            }
            
            public static IconInjector.IconFile FromArray(byte[] array)
            {
                IconInjector.IconFile iconFile = new IconInjector.IconFile();
                GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                iconFile.iconDir = (IconInjector.ICONDIR)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(IconInjector.ICONDIR));
                iconFile.iconEntry = new IconInjector.ICONDIRENTRY[System.Convert.ToInt32((iconFile.iconDir.Count - 1 + 1)) - 1 + 1];
                iconFile.iconImage = new byte[System.Convert.ToInt32((iconFile.iconDir.Count - 1 + 1)) - 1 + 1][];
                int num = Marshal.SizeOf(iconFile.iconDir);
                Type typeFromHandle = typeof(IconInjector.ICONDIRENTRY);
                int num2 = Marshal.SizeOf(typeFromHandle);
                int num3 = 0;
                int num4 = System.Convert.ToInt32((iconFile.iconDir.Count - 1));
                int num5 = num3;
                while (true)
                {
                    int num6 = num5;
                    int num7 = num4;
                    if (num6 > num7)
                        break;
                    IntPtr ptr = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + System.Convert.ToInt64(num));
                    IconInjector.ICONDIRENTRY icondirentry = (IconInjector.ICONDIRENTRY)Marshal.PtrToStructure(ptr, typeFromHandle);
                    iconFile.iconEntry[num5] = icondirentry;
                    iconFile.iconImage[num5] = new byte[icondirentry.BytesInRes - 1 + 1 - 1 + 1];
                    Buffer.BlockCopy(array, icondirentry.ImageOffset, iconFile.iconImage[num5], 0, icondirentry.BytesInRes);
                    num += num2;
                    num5 += 1;
                }
                gchandle.Free();
                return iconFile;
            }

            // Token: 0x06000029 RID: 41 RVA: 0x0011F7E8 File Offset: 0x0011DBE8
            public static IconInjector.IconFile FromFile(string filename)
            {
                IconInjector.IconFile iconFile = new IconInjector.IconFile();
                byte[] array = File.ReadAllBytes(filename);
                GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                iconFile.iconDir = (IconInjector.ICONDIR)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(IconInjector.ICONDIR));
                iconFile.iconEntry = new IconInjector.ICONDIRENTRY[System.Convert.ToInt32((iconFile.iconDir.Count - 1 + 1)) - 1 + 1];
                iconFile.iconImage = new byte[System.Convert.ToInt32((iconFile.iconDir.Count - 1 + 1)) - 1 + 1][];
                int num = Marshal.SizeOf(iconFile.iconDir);
                Type typeFromHandle = typeof(IconInjector.ICONDIRENTRY);
                int num2 = Marshal.SizeOf(typeFromHandle);
                int num3 = 0;
                int num4 = System.Convert.ToInt32((iconFile.iconDir.Count - 1));
                int num5 = num3;
                while (true)
                {
                    int num6 = num5;
                    int num7 = num4;
                    if (num6 > num7)
                        break;
                    IntPtr ptr = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + System.Convert.ToInt64(num));
                    IconInjector.ICONDIRENTRY icondirentry = (IconInjector.ICONDIRENTRY)Marshal.PtrToStructure(ptr, typeFromHandle);
                    iconFile.iconEntry[num5] = icondirentry;
                    iconFile.iconImage[num5] = new byte[icondirentry.BytesInRes - 1 + 1 - 1 + 1];
                    Buffer.BlockCopy(array, icondirentry.ImageOffset, iconFile.iconImage[num5], 0, icondirentry.BytesInRes);
                    num += num2;
                    num5 += 1;
                }
                gchandle.Free();
                return iconFile;
            }

            // Token: 0x0600002A RID: 42 RVA: 0x0011F948 File Offset: 0x0011DD48
            public byte[] CreateIconGroupData(uint iconBaseID)
            {
                // The following expression was wrapped in a checked-statement
                int num = Marshal.SizeOf(typeof(IconInjector.ICONDIR)) + Marshal.SizeOf(typeof(IconInjector.GRPICONDIRENTRY)) * this.ImageCount;
                byte[] array = new byte[num - 1 + 1 - 1 + 1];
                GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                Marshal.StructureToPtr(this.iconDir, gchandle.AddrOfPinnedObject(), false);
                int num2 = Marshal.SizeOf(this.iconDir);
                int num3 = 0;
                int num4 = this.ImageCount - 1;
                int num5 = num3;
                while (true)
                {
                    int num6 = num5;
                    int num7 = num4;
                    if (num6 > num7)
                        break;
                    IconInjector.GRPICONDIRENTRY grpicondirentry = default(GRPICONDIRENTRY);
                    IconInjector.BITMAPINFOHEADER bitmapinfoheader = default(BITMAPINFOHEADER);
                    GCHandle gchandle2 = GCHandle.Alloc(bitmapinfoheader, GCHandleType.Pinned);
                    Marshal.Copy(this.ImageData(num5), 0, gchandle2.AddrOfPinnedObject(), Marshal.SizeOf(typeof(IconInjector.BITMAPINFOHEADER)));
                    gchandle2.Free();
                    grpicondirentry.Width = this.iconEntry[num5].Width;
                    grpicondirentry.Height = this.iconEntry[num5].Height;
                    grpicondirentry.ColorCount = this.iconEntry[num5].ColorCount;
                    grpicondirentry.Reserved = this.iconEntry[num5].Reserved;
                    grpicondirentry.Planes = bitmapinfoheader.Planes;
                    grpicondirentry.BitCount = bitmapinfoheader.BitCount;
                    grpicondirentry.BytesInRes = this.iconEntry[num5].BytesInRes;
                    grpicondirentry.ID = System.Convert.ToUInt16((System.Convert.ToUInt64(iconBaseID) + System.Convert.ToUInt64((System.Convert.ToInt64(num5)))));
                    object structure = grpicondirentry;
                    IntPtr ptr = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + System.Convert.ToInt64(num2));
                    Marshal.StructureToPtr(structure, ptr, false);
                    num2 += Marshal.SizeOf(typeof(IconInjector.GRPICONDIRENTRY));
                    num5 += 1;
                }
                gchandle.Free();
                return array;
            }

            // Token: 0x04000028 RID: 40
            private IconInjector.ICONDIR iconDir;

            // Token: 0x04000029 RID: 41
            private IconInjector.ICONDIRENTRY[] iconEntry;

            // Token: 0x0400002A RID: 42
            private byte[][] iconImage;
        }
    }
#pragma warning restore 649
}
