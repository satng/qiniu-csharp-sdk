﻿
using System;
using System.IO;
namespace Qiniu.Util
{
    public class QETag
    {
        private static int BLOCK_SIZE = 4 * 1024 * 1024;
        private static int BLOCK_SHA1_SIZE = 20;
        public static string hash(string filePath)
        {
            string qetag = "";
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    long fileLength = stream.Length;
                    byte[] buffer = new byte[BLOCK_SIZE];
                    byte[] finalBuffer = new byte[BLOCK_SHA1_SIZE + 1];
                    if (fileLength <= BLOCK_SIZE)
                    {
                        int readByteCount = stream.Read(buffer, 0, BLOCK_SIZE);
                        byte[] readBuffer = new byte[readByteCount];
                        Array.Copy(buffer, readBuffer, readByteCount);

                        byte[] sha1Buffer = StringUtils.sha1(readBuffer);

                        finalBuffer[0] = 0x16;
                        Array.Copy(sha1Buffer, 0, finalBuffer, 1, sha1Buffer.Length);
                    }
                    else
                    {
                        long blockCount = (fileLength % BLOCK_SIZE == 0) ? (fileLength / BLOCK_SIZE) : (fileLength / BLOCK_SIZE + 1);
                        byte[] sha1AllBuffer = new byte[BLOCK_SHA1_SIZE * blockCount];

                        for (int i = 0; i < blockCount; i++)
                        {
                            int readByteCount = stream.Read(buffer, 0, BLOCK_SIZE);
                            byte[] readBuffer = new byte[readByteCount];
                            Array.Copy(buffer, readBuffer, readByteCount);

                            byte[] sha1Buffer = StringUtils.sha1(readBuffer);
                            Array.Copy(sha1Buffer, 0, sha1AllBuffer, i * BLOCK_SHA1_SIZE, sha1Buffer.Length);
                        }

                        byte[] sha1AllBufferSha1 = StringUtils.sha1(sha1AllBuffer);

                        finalBuffer[0] = 0x96;
                        Array.Copy(sha1AllBufferSha1, 0, finalBuffer, 1, sha1AllBufferSha1.Length);

                    }
                    qetag = StringUtils.urlSafeBase64Encode(finalBuffer);
                }
            }
            catch (Exception) { }

            return qetag;
        }
    }
}