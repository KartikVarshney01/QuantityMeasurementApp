using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace QuantityMeasurementAppRepoLayer.Utilities
{
    // Manages a pool of reusable SqlConnection objects.
    // The DatabaseRepository borrows a connection when it needs one and returns it when done so connections are reused.
    public class ConnectionPool : IDisposable
    {
        private static ConnectionPool instance;
        private static readonly object lockObject = new object();

        // Stack to store connection objects
        private Stack<SqlConnection> availableConnections;
        private int totalCreated;
        private int maxPoolSize;
        private string connectionString;

        // Constructor
        private ConnectionPool(ApplicationConfig config)
        {
            connectionString = config.GetConnectionString();
            maxPoolSize = config.GetMaxPoolSize();
            availableConnections = new Stack<SqlConnection>();
            totalCreated = 0;

            Console.WriteLine("[ConnectionPool] Initializing. Max size: " + maxPoolSize);

            // Pre-open 2 connections at startup so first calls are fast
            int initialSize = Math.Min(2, maxPoolSize);
            for (int i = 0; i < initialSize; i++)
            {
                SqlConnection conn = CreateNewConnection();
                availableConnections.Push(conn);
            }

            Console.WriteLine("[ConnectionPool] Ready. Pre-created: " + initialSize);
        }

        // Method to get instance (Singleton pattern)
        public static ConnectionPool GetInstance(ApplicationConfig config)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new ConnectionPool(config);
                    }
                }
            }
            return instance;
        }

        // Method to open and returns a brand new SqlConnection
        private SqlConnection CreateNewConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            totalCreated++;
            return conn;
        }

        // Method to borrow a connection from the pool
        public SqlConnection GetConnection()
        {
            lock (lockObject)
            {
                if (availableConnections.Count > 0)
                {
                    SqlConnection conn = availableConnections.Pop();
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    return conn;
                }

                if (totalCreated < maxPoolSize)
                {
                    Console.WriteLine("[ConnectionPool] Creating new connection. Total: "
                        + (totalCreated + 1));
                    return CreateNewConnection();
                }

                // Pool exhausted so wait briefly then retry
                Console.WriteLine("[ConnectionPool] Pool exhausted. Waiting 500ms...");
                System.Threading.Thread.Sleep(500);

                if (availableConnections.Count > 0)
                {
                    return availableConnections.Pop();
                }

                // Create emergency connection beyond pool limit
                Console.WriteLine("[ConnectionPool] Creating emergency connection.");
                return CreateNewConnection();
            }
        }

        // Method to return a borrowed connection back to the pool
        public void ReturnConnection(SqlConnection conn)
        {
            if (conn == null)
            {
                return;
            }

            lock (lockObject)
            {
                if (availableConnections.Count < maxPoolSize
                    && conn.State == ConnectionState.Open)
                {
                    // Return to pool for reuse
                    availableConnections.Push(conn);
                }
                else
                {
                    // Pool is full or connection is broken then close and dispose
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        // Method to return a human readable summary of the pool state
        public string GetPoolStatistics()
        {
            return "Total created: " + totalCreated
                + " | Idle: " + availableConnections.Count
                + " | Max: " + maxPoolSize;
        }

        // Method to close all connections in the pool
        public void Dispose()
        {
            lock (lockObject)
            {
                Console.WriteLine("[ConnectionPool] Disposing all connections...");
                while (availableConnections.Count > 0)
                {
                    SqlConnection conn = availableConnections.Pop();
                    conn.Close();
                    conn.Dispose();
                }
                Console.WriteLine("[ConnectionPool] All connections disposed.");
            }
        }

        // Method to reset singleton instance (for testing purposes)
        public static void Reset()
        {
            lock (lockObject)
            {
                if (instance != null)
                {
                    instance.Dispose();
                    instance = null;
                }
            }
        }
    }
}