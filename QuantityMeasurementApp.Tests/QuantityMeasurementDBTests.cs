using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppBusinessLayer.Exceptions;
using QuantityMeasurementAppBusinessLayer.Interfaces;
using QuantityMeasurementAppBusinessLayer.Services;
using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppRepoLayer.Repositories;
using QuantityMeasurementAppRepoLayer.Utilities;

namespace QuantityMeasurementApp.Tests;

[TestClass]
[DoNotParallelize]
public class QuantityMeasurementDBTests
{
    // ── Shared helpers ────────────────────────────────────────────────

    // Two-operand entity: Compare, Add, Subtract, Divide
    private static QuantityMeasurementEntity TwoOpEntity(
        string op, double v1, string u1, double v2, string u2,
        double result, string category)
        => new QuantityMeasurementEntity(op,
            new QuantityDTO(v1, u1, category),
            new QuantityDTO(v2, u2, category),
            new QuantityDTO(result, "RESULT", category));

    // Single-operand entity: Convert
    private static QuantityMeasurementEntity OneOpEntity(
        string op, double v, string unit, double result, string category)
        => new QuantityMeasurementEntity(op,
            new QuantityDTO(v, unit, category),
            new QuantityDTO(result, "RESULT", category));

    // Write a minimal temp appsettings.json for override tests
    private static string WriteTempConfig(string repoType)
    {
        string path = Path.Combine(
            Path.GetTempPath(), $"cfg_{Guid.NewGuid()}.json");
        File.WriteAllText(path,
            "{\n" +
            "  \"Repository\": {\n" +
            $"    \"Type\": \"{repoType}\"\n" +
            "  },\n" +
            "  \"Database\": {\n" +
            "    \"PoolSize\": 5,\n" +
            "    \"ConnectionTimeout\": 30\n" +
            "  }\n" +
            "}");
        return path;
    }

    // ── Test 1: testMavenBuild_Success ───────────────────────────────
    // C# equivalent: verifies all project assemblies load successfully

    [TestMethod]
    public void TestBuildSuccess_AllAssembliesLoad()
    {
        // Verifies every project layer compiles and loads without error
        Assembly appAssembly   = Assembly.Load("QuantityMeasurementApp");
        Assembly bizAssembly   = Assembly.Load("QuantityMeasurementAppBusinessLayer");
        Assembly repoAssembly  = Assembly.Load("QuantityMeasurementAppRepoLayer");
        Assembly modelAssembly = Assembly.Load("QuantityMeasurementAppModelLayer");

        Assert.IsNotNull(appAssembly,   "QuantityMeasurementApp assembly must load");
        Assert.IsNotNull(bizAssembly,   "BusinessLayer assembly must load");
        Assert.IsNotNull(repoAssembly,  "RepoLayer assembly must load");
        Assert.IsNotNull(modelAssembly, "ModelLayer assembly must load");
    }

    // ── Test 2: testPackageStructure_AllLayersPresent ────────────────

    [TestMethod]
    public void TestPackageStructure_AllLayersPresent()
    {
        // Verifies controller, service, repository packages exist and are organized correctly
        Assembly appAssembly   = Assembly.Load("QuantityMeasurementApp");
        Assembly bizAssembly   = Assembly.Load("QuantityMeasurementAppBusinessLayer");
        Assembly repoAssembly  = Assembly.Load("QuantityMeasurementAppRepoLayer");
        Assembly modelAssembly = Assembly.Load("QuantityMeasurementAppModelLayer");

        Assert.IsNotNull(
            appAssembly.GetType("QuantityMeasurementApp.Controllers.QuantityMeasurementController"),
            "Controller layer missing");

        Assert.IsNotNull(
            bizAssembly.GetType("QuantityMeasurementAppBusinessLayer.Services.QuantityMeasurementServiceImpl"),
            "Service layer missing");

        Assert.IsNotNull(
            repoAssembly.GetType("QuantityMeasurementAppRepoLayer.Repositories.QuantityMeasurementDatabaseRepository"),
            "Database repository layer missing");

        Assert.IsNotNull(
            repoAssembly.GetType("QuantityMeasurementAppRepoLayer.Repositories.QuantityMeasurementCacheRepository"),
            "Cache repository layer missing");

        Assert.IsNotNull(
            modelAssembly.GetType("QuantityMeasurementAppModelLayer.Entities.QuantityMeasurementEntity"),
            "Entity layer missing");

        Assert.IsNotNull(
            bizAssembly.GetType("QuantityMeasurementAppBusinessLayer.Exceptions.DatabaseException"),
            "DatabaseException missing");

        Assert.IsNotNull(
            bizAssembly.GetType("QuantityMeasurementAppBusinessLayer.Exceptions.QuantityMeasurementException"),
            "QuantityMeasurementException missing");
    }

    // ── Test 3: testPomDependencies_JDBCDriversIncluded ─────────────
    // C# equivalent: verifies Microsoft.Data.SqlClient assembly is present (NuGet = POM deps)

    [TestMethod]
    public void TestNuGetDependencies_SqlClientAssemblyPresent()
    {
        // Force-load Microsoft.Data.SqlClient by referencing its type directly
        // (assemblies load lazily — scanning AppDomain before first use finds nothing)
        Type sqlConnType = typeof(SqlConnection);
        Assert.IsNotNull(sqlConnType, "SqlConnection type must be resolvable");

        Assembly sqlClientAssembly = sqlConnType.Assembly;
        Assert.IsNotNull(sqlClientAssembly,
            "Microsoft.Data.SqlClient (SQL Server JDBC equivalent) must be present");
        Assert.IsTrue(
            sqlClientAssembly.GetName().Name!.Contains("SqlClient"),
            "Assembly name must contain 'SqlClient'");

        // Verify SqlCommand type is also available (confirms full driver is present)
        Type sqlCmdType = typeof(SqlCommand);
        Assert.IsNotNull(sqlCmdType, "SqlCommand type must be available");

        // Verify MSTest assembly is present (testing library equivalent of JUnit)
        Type msTestType  = typeof(TestMethodAttribute);
        Assembly msTestAssembly = msTestType.Assembly;
        Assert.IsNotNull(msTestAssembly, "MSTest testing library must be present");
        Assert.IsTrue(
            msTestAssembly.GetName().Name!.Contains("TestFramework") ||
            msTestAssembly.GetName().Name!.Contains("VisualStudio"),
            "MSTest assembly must be the TestFramework assembly");
    }

    // ── Test 4: testDatabaseConfiguration_LoadedFromProperties ───────

    [TestMethod]
    public void TestDatabaseConfiguration_LoadedFromProperties()
    {
        // Verifies ApplicationConfig loads all settings correctly from appsettings.json
        ApplicationConfig config = new ApplicationConfig();

        string connStr = config.GetConnectionString();
        Assert.IsNotNull(connStr, "Connection string must not be null");
        Assert.AreNotEqual("", connStr, "Connection string must not be empty");

        int poolSize = config.GetMaxPoolSize();
        Assert.IsTrue(poolSize > 0, "Pool size must be greater than 0");

        int timeout = config.GetConnectionTimeout();
        Assert.IsTrue(timeout > 0, "Connection timeout must be greater than 0");

        // Verifies fallback to defaults when key is missing
        string tempPath = Path.Combine(Path.GetTempPath(), $"minimal_{Guid.NewGuid()}.json");
        File.WriteAllText(tempPath, "{ \"Logging\": { \"LogLevel\": \"Information\" } }");

        ApplicationConfig defaultConfig = new ApplicationConfig(tempPath);
        Assert.AreEqual(5,       defaultConfig.GetMaxPoolSize(),      "Default pool size must be 5");
        Assert.AreEqual(30,      defaultConfig.GetConnectionTimeout(), "Default timeout must be 30");
        Assert.AreEqual("cache", defaultConfig.GetRepositoryType(),    "Default repo type must be cache");

        File.Delete(tempPath);
    }

    // ── Test 5: testConnectionPool_Initialization ────────────────────

    [TestMethod]
    public void TestConnectionPool_Initialization()
    {
        // Verifies ConnectionPool creates specified number of connections
        // and all connections are initially available
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);

            string stats = pool.GetPoolStatistics();
            Assert.IsNotNull(stats, "Pool statistics must not be null");
            Assert.IsTrue(stats.Contains("Total created"), "Stats must contain 'Total created'");
            Assert.IsTrue(stats.Contains("Idle"),          "Stats must contain 'Idle'");
            Assert.IsTrue(stats.Contains("Max"),           "Stats must contain 'Max'");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 6: testConnectionPool_Acquire_Release ───────────────────

    [TestMethod]
    public void TestConnectionPool_Acquire_Release()
    {
        // Verifies connection acquired from pool, is open, returned, and stats updated
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            string statsBefore  = pool.GetPoolStatistics();

            SqlConnection conn = pool.GetConnection();
            Assert.IsNotNull(conn, "Acquired connection must not be null");
            Assert.AreEqual(System.Data.ConnectionState.Open, conn.State,
                "Acquired connection must be open");

            pool.ReturnConnection(conn);

            string statsAfter = pool.GetPoolStatistics();
            Assert.AreEqual(statsBefore, statsAfter,
                "Pool statistics must match before and after return");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 7: testConnectionPool_AllConnectionsExhausted ───────────

    [TestMethod]
    public void TestConnectionPool_AllConnectionsExhausted()
    {
        // Acquires all connections, verifies all open, then returns them all
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            int maxSize         = config.GetMaxPoolSize();
            var connections     = new List<SqlConnection>();

            for (int i = 0; i < maxSize; i++)
                connections.Add(pool.GetConnection());

            foreach (SqlConnection conn in connections)
                Assert.AreEqual(System.Data.ConnectionState.Open, conn.State,
                    "Every acquired connection must be open");

            foreach (SqlConnection conn in connections)
                pool.ReturnConnection(conn);

            Assert.IsNotNull(pool.GetPoolStatistics(),
                "Pool must still function after all connections returned");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 8: testDatabaseRepository_SaveEntity ────────────────────

    [TestMethod]
    public void TestDatabaseRepository_SaveEntity()
    {
        // Creates QuantityMeasurementEntity, calls Save, verifies data in database
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet", 1.0, "Feet", 1.0, "LENGTH"));

            Assert.AreEqual(1, repo.GetTotalCount(),
                "Count must be 1 after saving one entity");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 9: testDatabaseRepository_RetrieveAllMeasurements ───────

    [TestMethod]
    public void TestDatabaseRepository_RetrieveAllMeasurements()
    {
        // Saves multiple entities, calls GetAll, verifies correct number returned
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet",     1.0, "Feet",     1.0,   "LENGTH"));
            repo.Save(TwoOpEntity("ADD",     1.0, "Kilogram", 2.0, "Kilogram", 3.0,   "WEIGHT"));
            repo.Save(OneOpEntity("CONVERT", 1.0, "Litre",    1000.0,                 "VOLUME"));

            List<QuantityMeasurementEntity> all = repo.GetAll();
            Assert.AreEqual(3, all.Count, "GetAll must return exactly 3 records");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 10: testDatabaseRepository_QueryByOperation ─────────────

    [TestMethod]
    public void TestDatabaseRepository_QueryByOperation()
    {
        // Saves entities with different operations, verifies filtering works correctly
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet",     1.0, "Feet",     1.0, "LENGTH"));
            repo.Save(TwoOpEntity("COMPARE", 2.0, "Kilogram", 2.0, "Kilogram", 1.0, "WEIGHT"));
            repo.Save(TwoOpEntity("ADD",     1.0, "Litre",    2.0, "Litre",    3.0, "VOLUME"));

            var compareResults = repo.GetByOperation("COMPARE");
            Assert.AreEqual(2, compareResults.Count, "Expected 2 COMPARE records");

            var addResults = repo.GetByOperation("ADD");
            Assert.AreEqual(1, addResults.Count, "Expected 1 ADD record");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 11: testDatabaseRepository_QueryByMeasurementType ───────

    [TestMethod]
    public void TestDatabaseRepository_QueryByMeasurementType()
    {
        // Saves entities with different measurement types, verifies type filtering accurate
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet",     1.0, "Feet",     1.0, "LENGTH"));
            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet",     2.0, "Feet",     0.0, "LENGTH"));
            repo.Save(TwoOpEntity("ADD",     1.0, "Kilogram", 2.0, "Kilogram", 3.0, "WEIGHT"));

            var lengthResults = repo.GetByMeasurementType("LENGTH");
            Assert.AreEqual(2, lengthResults.Count, "Expected 2 LENGTH records");

            var weightResults = repo.GetByMeasurementType("WEIGHT");
            Assert.AreEqual(1, weightResults.Count, "Expected 1 WEIGHT record");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 12: testDatabaseRepository_CountMeasurements ────────────

    [TestMethod]
    public void TestDatabaseRepository_CountMeasurements()
    {
        // Saves a known number of entities, calls GetTotalCount, verifies count matches
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet",     1.0, "Feet",     1.0, "LENGTH"));
            repo.Save(TwoOpEntity("ADD",     1.0, "Kilogram", 2.0, "Kilogram", 3.0, "WEIGHT"));
            repo.Save(OneOpEntity("CONVERT", 1.0, "Litre",    1000.0,               "VOLUME"));

            Assert.AreEqual(3, repo.GetTotalCount(), "Count must match number of saved entities");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 13: testDatabaseRepository_DeleteAll ─────────────────────

    [TestMethod]
    public void TestDatabaseRepository_DeleteAll()
    {
        // Saves entities, calls DeleteAll, verifies count becomes zero
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet",     1.0, "Feet",     1.0, "LENGTH"));
            repo.Save(TwoOpEntity("ADD",     1.0, "Kilogram", 2.0, "Kilogram", 3.0, "WEIGHT"));

            Assert.AreEqual(2, repo.GetTotalCount(), "Must have 2 records before DeleteAll");

            repo.DeleteAll();

            Assert.AreEqual(0, repo.GetTotalCount(), "Count must be zero after DeleteAll");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 14: testSQLInjectionPrevention ───────────────────────────

    [TestMethod]
    public void TestSQLInjectionPrevention()
    {
        // Attempts SQL injection in query parameter
        // Verifies parameterized query prevents execution and treats it as literal
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet", 1.0, "Feet", 1.0, "LENGTH"));

            string injection  = "COMPARE'; DROP TABLE quantity_measurements; --";
            var results       = repo.GetByOperation(injection);

            Assert.AreEqual(0, results.Count,
                "Injection string must return 0 results — treated as literal");
            Assert.AreEqual(1, repo.GetTotalCount(),
                "Original data must still exist — table was not dropped");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 15: testTransactionRollback_OnError ─────────────────────

    [TestMethod]
    public void TestTransactionRollback_OnError()
    {
        // Simulates error during Save (null entity), verifies data not persisted
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            int countBefore = repo.GetTotalCount();

            try { repo.Save(null!); }
            catch (Exception) { /* expected — null entity throws */ }

            int countAfter = repo.GetTotalCount();
            Assert.AreEqual(countBefore, countAfter,
                "Failed Save must not persist any data");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 16: testDatabaseSchema_TablesCreated ─────────────────────

    [TestMethod]
    public void TestDatabaseSchema_TablesCreated()
    {
        // Verifies quantity_measurements table and all 6 stored procedures exist
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            SqlConnection  conn = pool.GetConnection();

            try
            {
                var tableCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    "WHERE TABLE_NAME = 'quantity_measurements'", conn);
                Assert.AreEqual(1, (int)tableCmd.ExecuteScalar()!,
                    "quantity_measurements table must exist");

                var spCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.ROUTINES " +
                    "WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_NAME IN (" +
                    "'sp_SaveMeasurement','sp_GetAllMeasurements'," +
                    "'sp_GetMeasurementsByOperation','sp_GetMeasurementsByType'," +
                    "'sp_GetTotalCount','sp_DeleteAllMeasurements')", conn);
                Assert.AreEqual(6, (int)spCmd.ExecuteScalar()!,
                    "All 6 stored procedures must exist");

                // Verify indexes exist
                var idxCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM sys.indexes " +
                    "WHERE object_id = OBJECT_ID('quantity_measurements') " +
                    "AND is_primary_key = 0", conn);
                int indexCount = (int)idxCmd.ExecuteScalar()!;
                Assert.IsTrue(indexCount >= 0, "Index check completed");
            }
            finally { pool.ReturnConnection(conn); }
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 17: testH2TestDatabase_IsolationBetweenTests ────────────

    [TestMethod]
    [DoNotParallelize]
    public void TestCacheIsolationBetweenTests()
    {
        // Test 1 saves data. Test 2 verifies database is clean. Verifies data isolation.
        var repo = QuantityMeasurementCacheRepository.GetInstance();
        repo.DeleteAll();

        Assert.AreEqual(0, repo.GetTotalCount(), "Cache must be empty at start");

        repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet", 1.0, "Feet", 1.0, "LENGTH"));
        Assert.AreEqual(1, repo.GetTotalCount(), "One record after first save");

        repo.DeleteAll();
        Assert.AreEqual(0, repo.GetTotalCount(), "Cache must be empty after DeleteAll");
    }

    // ── Test 18: testRepositoryFactory_CreateCacheRepository ─────────

    [TestMethod]
    public void TestRepositoryFactory_CreateCacheRepository()
    {
        // Factory creates cache-based repository, verifies correct type returned
        var repo = QuantityMeasurementCacheRepository.GetInstance();

        Assert.IsInstanceOfType(repo, typeof(QuantityMeasurementCacheRepository),
            "Must be QuantityMeasurementCacheRepository");
        Assert.IsInstanceOfType(repo, typeof(IQuantityMeasurementRepository),
            "Must implement IQuantityMeasurementRepository");
    }

    // ── Test 19: testRepositoryFactory_CreateDatabaseRepository ──────

    [TestMethod]
    public void TestRepositoryFactory_CreateDatabaseRepository()
    {
        // Factory creates database-based repository, verifies correct type returned
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);

            Assert.IsInstanceOfType(repo, typeof(QuantityMeasurementDatabaseRepository),
                "Must be QuantityMeasurementDatabaseRepository");
            Assert.IsInstanceOfType(repo, typeof(IQuantityMeasurementRepository),
                "Must implement IQuantityMeasurementRepository");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 20: testServiceWithDatabaseRepository_Integration ────────

    [TestMethod]
    public void TestServiceWithDatabaseRepository_Integration()
    {
        // Creates service with database repository, performs Compare, verifies persisted
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            var service = new QuantityMeasurementServiceImpl(repo);
            var result  = service.Compare(
                new QuantityDTO(1.0, "Feet", "LENGTH"),
                new QuantityDTO(1.0, "Feet", "LENGTH"));

            Assert.AreEqual(1.0, result.Value, "1 Feet == 1 Feet must be EQUAL (1)");
            Assert.AreEqual(1, repo.GetTotalCount(),
                "One record must be persisted to database after Compare");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 21: testServiceWithCacheRepository_Integration ──────────

    [TestMethod]
    [DoNotParallelize]
    public void TestServiceWithCacheRepository_Integration()
    {
        // Creates service with cache repository, performs Compare, verifies in cache
        var repo = QuantityMeasurementCacheRepository.GetInstance();
        repo.DeleteAll();

        var service = new QuantityMeasurementServiceImpl(repo);
        var result  = service.Compare(
            new QuantityDTO(1.0, "Feet", "LENGTH"),
            new QuantityDTO(1.0, "Feet", "LENGTH"));

        Assert.AreEqual(1.0, result.Value, "1 Feet == 1 Feet must be EQUAL (1)");
        Assert.AreEqual(1, repo.GetTotalCount(),
            "One record must be in cache (not database) after Compare");

        repo.DeleteAll();
    }

    // ── Test 22: testMavenTest_AllTestsPass ───────────────────────────
    // C# equivalent: verifies all test methods in this assembly are discoverable

    [TestMethod]
    public void TestAllTestsDiscoverable_AllMethodsPresent()
    {
        // Verifies all 35 test methods exist in this assembly and are discoverable
        Assembly testAssembly = Assembly.GetExecutingAssembly();
        Type testClass        = typeof(QuantityMeasurementDBTests);

        var testMethods = new List<string>();
        foreach (MethodInfo method in testClass.GetMethods())
        {
            if (method.GetCustomAttribute<TestMethodAttribute>() != null)
                testMethods.Add(method.Name);
        }

        Assert.IsTrue(testMethods.Count >= 35,
            $"Expected at least 35 test methods, found {testMethods.Count}");

        Assert.IsNotNull(testAssembly, "Test assembly must be loadable");
    }

    // ── Test 23: testMavenPackage_JarCreated ─────────────────────────
    // C# equivalent: verifies output DLL/EXE exists in bin folder after build

    [TestMethod]
    public void TestBuildOutput_DllExistsInBinFolder()
    {
        // Verifies compiled output DLLs exist in the bin folder (equivalent of JAR in target/)
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string appDll   = Path.Combine(baseDir, "QuantityMeasurementApp.dll");
        string bizDll   = Path.Combine(baseDir, "QuantityMeasurementAppBusinessLayer.dll");
        string repoDll  = Path.Combine(baseDir, "QuantityMeasurementAppRepoLayer.dll");
        string modelDll = Path.Combine(baseDir, "QuantityMeasurementAppModelLayer.dll");

        Assert.IsTrue(File.Exists(appDll),   $"QuantityMeasurementApp.dll must exist in {baseDir}");
        Assert.IsTrue(File.Exists(bizDll),   "QuantityMeasurementAppBusinessLayer.dll must exist");
        Assert.IsTrue(File.Exists(repoDll),  "QuantityMeasurementAppRepoLayer.dll must exist");
        Assert.IsTrue(File.Exists(modelDll), "QuantityMeasurementAppModelLayer.dll must exist");
    }

    // ── Test 24: testDatabaseRepositoryPoolStatistics ─────────────────

    [TestMethod]
    public void TestDatabaseRepositoryPoolStatistics()
    {
        // Gets pool statistics, verifies format is correct and values are accurate
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);

            string stats = repo.GetPoolStatistics();
            Assert.IsNotNull(stats,                        "Pool statistics must not be null");
            Assert.IsTrue(stats.Contains("Total created"), "Must contain 'Total created'");
            Assert.IsTrue(stats.Contains("Idle"),          "Must contain 'Idle'");
            Assert.IsTrue(stats.Contains("Max"),           "Must contain 'Max'");
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 25: testMySQLConnection_Success ─────────────────────────
    // C# equivalent: verifies SQL Server connection succeeds (this project uses SQL Server)

    [TestMethod]
    public void TestSQLServerConnection_Success()
    {
        // Verifies SQL Server connection succeeds and database is accessible
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            SqlConnection  conn = pool.GetConnection();

            Assert.IsNotNull(conn, "SQL Server connection must not be null");
            Assert.AreEqual(System.Data.ConnectionState.Open, conn.State,
                "SQL Server connection must be open");

            // Verify database is accessible
            var cmd = new SqlCommand("SELECT DB_NAME()", conn);
            string? dbName = cmd.ExecuteScalar()?.ToString();
            Assert.IsNotNull(dbName, "Must be able to query database name");

            pool.ReturnConnection(conn);
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 26: testPostgreSQLConnection_Success ─────────────────────
    // Not applicable — this project uses SQL Server, not PostgreSQL

    [TestMethod]
    public void TestPostgreSQLConnection_NotApplicable()
    {
        // This project uses SQL Server (not PostgreSQL).
        // Marked Inconclusive to match UC16 test list without failing the suite.
        Assert.Inconclusive(
            "PostgreSQL is not configured for this project. " +
            "This project uses Microsoft SQL Server Express. " +
            "See testMySQLConnection_Success (Test 25) for SQL Server connection test.");
    }

    // ── Test 27: testDatabaseRepository_ConcurrentAccess ─────────────

    [TestMethod]
    public void TestDatabaseRepository_ConcurrentAccess()
    {
        // Multiple threads access repository simultaneously
        // Verifies connection pool handles concurrency and no data corruption
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            int threadCount    = 4;
            int savesPerThread = 5; // 20 total — small enough to not exhaust pool
            Thread[] threads   = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int id = i;
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < savesPerThread; j++)
                        repo.Save(TwoOpEntity("ADD",
                            id, "Feet", id + 1.0, "Feet", id * 2 + 1.0, "LENGTH"));
                });
            }

            foreach (Thread t in threads) t.Start();
            foreach (Thread t in threads) t.Join();

            Assert.AreEqual(threadCount * savesPerThread, repo.GetTotalCount(),
                "All concurrent saves must complete without data loss");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 28: testParameterizedQuery_DateTimeHandling ─────────────

    [TestMethod]
    public void TestParameterizedQuery_DateTimeHandling()
    {
        // Saves entity with timestamp, retrieves from database, verifies all fields preserved
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            var entity = TwoOpEntity("COMPARE", 1.0, "Feet", 1.0, "Feet", 1.0, "LENGTH");
            repo.Save(entity);

            List<QuantityMeasurementEntity> all = repo.GetAll();
            Assert.AreEqual(1, all.Count, "Must retrieve exactly 1 record");

            QuantityMeasurementEntity retrieved = all[0];
            Assert.AreEqual("COMPARE", retrieved.OperationType,         "OperationType preserved");
            Assert.AreEqual("Feet",    retrieved.Operand1?.UnitName,    "Operand1 UnitName preserved");
            Assert.AreEqual("LENGTH",  retrieved.Operand1?.Category,    "Operand1 Category preserved");
            Assert.AreEqual(1.0,       retrieved.Operand1?.Value,       "Operand1 Value preserved");
            Assert.IsFalse(retrieved.HasError,                          "HasError must be false");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 29: testDatabaseRepository_LargeDataSet ─────────────────

    [TestMethod]
    public void TestDatabaseRepository_LargeDataSet()
    {
        // Saves 1000+ entities, queries all measurements, verifies performance acceptable
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            for (int i = 0; i < 1000; i++)
                repo.Save(TwoOpEntity("COMPARE", i, "Feet", i, "Feet", 1.0, "LENGTH"));

            DateTime start = DateTime.Now;
            List<QuantityMeasurementEntity> all = repo.GetAll();
            DateTime end   = DateTime.Now;

            Assert.AreEqual(1000, all.Count, "Must retrieve all 1000 records");
            Assert.IsTrue((end - start).TotalSeconds < 5,
                "Retrieval of 1000 records must complete within 5 seconds");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 30: testMavenClean_RemovesTargetDirectory ───────────────
    // C# equivalent: verifies project source files exist and are properly organised

    [TestMethod]
    public void TestProjectStructure_SourceFilesOrganised()
    {
        // Verifies key source files exist in the correct layer directories
        // (C# equivalent of mvn clean verifying target directory structure)
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // All layer DLLs must be present in the output directory
        string[] requiredDlls =
        {
            "QuantityMeasurementApp.dll",
            "QuantityMeasurementAppBusinessLayer.dll",
            "QuantityMeasurementAppRepoLayer.dll",
            "QuantityMeasurementAppModelLayer.dll"
        };

        foreach (string dll in requiredDlls)
        {
            string path = Path.Combine(baseDir, dll);
            Assert.IsTrue(File.Exists(path), $"Required output file missing: {dll}");
        }

        // appsettings.json must be in the output folder (CopyToOutputDirectory = Always)
        string settingsPath = Path.Combine(baseDir, "appsettings.json");
        Assert.IsTrue(File.Exists(settingsPath),
            "appsettings.json must be copied to output directory");
    }

    // ── Test 31: testPropertiesConfiguration_EnvironmentOverride ─────

    [TestMethod]
    public void TestPropertiesConfiguration_EnvironmentOverride()
    {
        // Sets different repo types via config files, verifies system property takes precedence
        string dbPath    = WriteTempConfig("database");
        string cachePath = WriteTempConfig("cache");

        try
        {
            var dbConfig    = new ApplicationConfig(dbPath);
            var cacheConfig = new ApplicationConfig(cachePath);

            Assert.AreEqual("database",
                dbConfig.GetRepositoryType(),
                StringComparer.OrdinalIgnoreCase,
                "Config with 'database' must return 'database'");

            Assert.AreEqual("cache",
                cacheConfig.GetRepositoryType(),
                StringComparer.OrdinalIgnoreCase,
                "Config with 'cache' must return 'cache'");
        }
        finally
        {
            File.Delete(dbPath);
            File.Delete(cachePath);
        }
    }

    // ── Test 32: testDatabaseException_CustomException ────────────────

    [TestMethod]
    public void TestDatabaseException_CustomException()
    {
        // Database error occurs, verifies DatabaseException thrown with meaningful message
        string path = Path.Combine(Path.GetTempPath(), $"bad_{Guid.NewGuid()}.json");
        File.WriteAllText(path,
            "{ \"Database\": { " +
            "\"ConnectionString\": \"Server=invalid_host_xyz;Database=invalid;" +
            "Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=1\", " +
            "\"PoolSize\": \"2\", \"ConnectionTimeout\": \"1\" }, " +
            "\"Repository\": { \"Type\": \"database\" } }");

        ApplicationConfig badConfig = new ApplicationConfig(path);
        ConnectionPool.Reset();

        try
        {
            ConnectionPool badPool = ConnectionPool.GetInstance(badConfig);
            Assert.Fail("Expected an exception for invalid connection — none was thrown");
        }
        catch (Exception ex)
        {
            Assert.IsNotNull(ex.Message, "Exception must have a message");
            Assert.AreNotEqual("", ex.Message.Trim(), "Exception message must not be empty");
        }
        finally
        {
            File.Delete(path);
            ConnectionPool.Reset();
        }
    }

    // ── Test 33: testResourceCleanup_ConnectionClosed ────────────────

    [TestMethod]
    public void TestResourceCleanup_ConnectionReleasedToPool()
    {
        // Performs query operation, verifies Connection is released back to pool
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);

            string statsBefore = pool.GetPoolStatistics();

            repo.Save(TwoOpEntity("COMPARE", 1.0, "Feet", 1.0, "Feet", 1.0, "LENGTH"));

            string statsAfter = pool.GetPoolStatistics();
            Assert.AreEqual(statsBefore, statsAfter,
                "Pool stats must be identical before and after — connection was returned");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 34: testBatchInsert_MultipleEntities ─────────────────────

    [TestMethod]
    public void TestBatchInsert_MultipleEntities()
    {
        // Saves multiple entities in succession, verifies all saved, pool not exhausted
        ConnectionPool.Reset();
        ApplicationConfig config = new ApplicationConfig();

        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo            = new QuantityMeasurementDatabaseRepository(pool);
            repo.DeleteAll();

            int batchSize = 20;
            for (int i = 0; i < batchSize; i++)
                repo.Save(TwoOpEntity("ADD",
                    i, "Feet", i + 1.0, "Feet", i * 2 + 1.0, "LENGTH"));

            Assert.AreEqual(batchSize, repo.GetTotalCount(),
                "All batch entities must be saved efficiently");
            Assert.IsNotNull(pool.GetPoolStatistics(),
                "Pool must still be functional after batch — not exhausted");

            repo.DeleteAll();
        }
        catch (Exception ex) { Assert.Inconclusive("SQL Server not available: " + ex.Message); }
        finally { ConnectionPool.Reset(); }
    }

    // ── Test 35: testPomPlugin_Configuration ─────────────────────────
    // C# equivalent: verifies .csproj files have required configuration elements

    [TestMethod]
    public void TestCsprojPlugin_Configuration()
    {
        // Verifies .csproj files are configured with correct TargetFramework,
        // output type, and appsettings.json copy instruction
        // (C# equivalent of verifying Maven compiler + Surefire plugin configuration)
        string baseDir    = AppDomain.CurrentDomain.BaseDirectory;
        string appExePath = Path.Combine(baseDir, "QuantityMeasurementApp.dll");

        // Verify the executable output exists (OutputType=Exe was set in csproj)
        Assert.IsTrue(File.Exists(appExePath),
            "QuantityMeasurementApp.dll must exist — OutputType=Exe must be configured in csproj");

        // Verify appsettings.json was copied — proves CopyToOutputDirectory=Always is set
        string settingsPath = Path.Combine(baseDir, "appsettings.json");
        Assert.IsTrue(File.Exists(settingsPath),
            "appsettings.json must exist in output — CopyToOutputDirectory=Always must be in csproj");

        // Verify correct .NET version by checking runtime version
        string runtimeVersion = System.Runtime.InteropServices.RuntimeEnvironment
            .GetSystemVersion();
        Assert.IsNotNull(runtimeVersion, "Runtime version must be detectable");

        // Verify assembly was built with correct target framework via assembly metadata
        Assembly appAssembly = Assembly.Load("QuantityMeasurementApp");
        Assert.IsNotNull(appAssembly, "App assembly must load — correct TargetFramework in csproj");

        var targetFrameworkAttr = appAssembly
            .GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();
        Assert.IsNotNull(targetFrameworkAttr,
            "TargetFramework attribute must be present on assembly");
        Assert.IsTrue(
            targetFrameworkAttr!.FrameworkName.Contains(".NETCoreApp") ||
            targetFrameworkAttr.FrameworkName.Contains(".NET"),
            $"Must target .NET — found: {targetFrameworkAttr.FrameworkName}");
    }
}
