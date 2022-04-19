using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Repository
{
    public class IdWorker
    {
        private static long workerId;
        private static long twepoch = 687888001020L;
        private static long sequence = 0L;
        private static int workerIdBits = 4;
        public static long maxWorkerId = -1L ^ -1L << workerIdBits;
        private static int sequenceBits = 10;
        private static int workerIdShift = sequenceBits;
        private static int timestampLeftShift = sequenceBits + workerIdBits;
        public static long sequenceMask = -1L ^ -1L << sequenceBits;
        private long lastTimestamp = -1L;
        private static object lockobj = new object();
        private static IdWorker instance;
        /// <summary>
        /// Machine code
        /// </summary>
        /// <param name="workerId"></param>
        private IdWorker(long workerId)
        {
            if (workerId > maxWorkerId || workerId < 0)
                throw new Exception(string.Format("worker Id can't be greater than {0} or less than 0 ", workerId));
            IdWorker.workerId = workerId;
        }

        public static IdWorker Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockobj)
                    {
                        if (instance == null)
                            instance = new IdWorker(1);
                    }
                }

                return instance;
            }
        }

        public long NextId()
        {
            lock (this)
            {
                long timestamp = timeGen();
                if (this.lastTimestamp == timestamp)
                {
                    IdWorker.sequence = (IdWorker.sequence + 1) & IdWorker.sequenceMask;
                    if (IdWorker.sequence == 0)
                    {
                        timestamp = tillNextMillis(this.lastTimestamp);
                    }
                }
                else
                {
                    IdWorker.sequence = 0;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception(string.Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds",
                        this.lastTimestamp - timestamp));
                }
                this.lastTimestamp = timestamp;
                long nextId = (timestamp - twepoch << timestampLeftShift) | IdWorker.workerId << IdWorker.workerIdShift | IdWorker.sequence;
                return nextId;
            }
        }

        /// <summary>
        /// get next timestamp Milliseconds
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long tillNextMillis(long lastTimestamp)
        {
            long timestamp = timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = timeGen();
            }
            return timestamp;
        }


        private long timeGen()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public long NextMilliseconds()
        {
            lock (this)
            {
                long timestamp = timeGen();
                if (timestamp == this.lastTimestamp)
                {
                    timestamp = tillNextMillis(this.lastTimestamp);
                }
                this.lastTimestamp = timestamp;
                return timestamp;
            }
        }
    }

}