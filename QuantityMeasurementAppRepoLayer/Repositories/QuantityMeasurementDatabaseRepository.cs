using System.Data;
using Microsoft.Data.SqlClient;
using QuantityMeasurementAppRepoLayer.Exceptions;
using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppBusinessLayer.Interfaces;
using QuantityMeasurementAppRepoLayer.Utilities;

namespace QuantityMeasurementAppRepoLayer.Repositories;

// UC16: SQL Server database repository using stored procedures + ADO.NET.
public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
{
    private readonly ConnectionPool _pool;

    public QuantityMeasurementDatabaseRepository(ConnectionPool connectionPool)
    {
        if (connectionPool == null)
            throw new ArgumentException("ConnectionPool cannot be null");

        _pool = connectionPool;
        Console.WriteLine("[DatabaseRepository] Initialized with ADO.NET + SQL Server.");
    }

    // ── Save ──────────────────────────────────────────────────────────

    public void Save(QuantityMeasurementEntity entity)
    {
        if (entity == null)
            throw new ArgumentException("Entity cannot be null");

        SqlConnection conn = _pool.GetConnection();
        try
        {
            SqlCommand cmd = new SqlCommand("sp_SaveMeasurement", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            AddParam(cmd, "@operation",       SqlDbType.NVarChar, 50,  entity.OperationType);
            AddParam(cmd, "@first_value",      SqlDbType.Float,        (object?)entity.Operand1?.Value    ?? DBNull.Value);
            AddParam(cmd, "@first_unit",       SqlDbType.NVarChar, 50, (object?)entity.Operand1?.UnitName ?? DBNull.Value);
            AddParam(cmd, "@second_value",     SqlDbType.Float, entity.Operand2?.Value ?? 0.0);
            AddParam(cmd, "@second_unit",      SqlDbType.NVarChar, 50, (object?)entity.Operand2?.UnitName ?? DBNull.Value);
            AddParam(cmd, "@result_value",     SqlDbType.Float,        (object?)entity.Result?.Value      ?? DBNull.Value);
            AddParam(cmd, "@measurement_type", SqlDbType.NVarChar, 50, (object?)entity.Operand1?.Category ?? DBNull.Value);
            AddParam(cmd, "@is_error",         SqlDbType.Bit,           entity.HasError);
            AddParam(cmd, "@error_message",    SqlDbType.NVarChar, 500,
                string.IsNullOrEmpty(entity.ErrorMessage) ? DBNull.Value : (object)entity.ErrorMessage);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            Console.WriteLine("[DatabaseRepository] Saved: " + entity.OperationType);
        }
        catch (SqlException ex)
        {
            throw new DatabaseException(
                "Save failed (SQL error " + ex.Number + "): " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Save failed: " + ex.Message, ex);
        }
        finally
        {
            _pool.ReturnConnection(conn);
        }
    }

    // ── GetAll ────────────────────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetAll()
    {
        SqlConnection conn = _pool.GetConnection();
        try
        {
            SqlCommand cmd = new SqlCommand("sp_GetAllMeasurements", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            var results = ReadEntities(cmd);
            cmd.Dispose();
            return results;
        }
        catch (SqlException ex)
        {
            throw new DatabaseException(
                "GetAll failed (SQL error " + ex.Number + "): " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("GetAll failed: " + ex.Message, ex);
        }
        finally { _pool.ReturnConnection(conn); }
    }

    public void Clear() => DeleteAll();

    // ── GetByOperation ────────────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetByOperation(string operation)
    {
        if (operation == null) throw new ArgumentException("Operation cannot be null");

        SqlConnection conn = _pool.GetConnection();
        try
        {
            SqlCommand cmd = new SqlCommand("sp_GetMeasurementsByOperation", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            AddParam(cmd, "@operation", SqlDbType.NVarChar, 50, operation);
            var results = ReadEntities(cmd);
            cmd.Dispose();
            return results;
        }
        catch (SqlException ex)
        {
            throw new DatabaseException(
                "GetByOperation failed (SQL error " + ex.Number + "): " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("GetByOperation failed: " + ex.Message, ex);
        }
        finally { _pool.ReturnConnection(conn); }
    }

    // ── GetByMeasurementType ──────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
    {
        if (measurementType == null) throw new ArgumentException("MeasurementType cannot be null");

        SqlConnection conn = _pool.GetConnection();
        try
        {
            SqlCommand cmd = new SqlCommand("sp_GetMeasurementsByType", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            AddParam(cmd, "@measurement_type", SqlDbType.NVarChar, 50, measurementType);
            var results = ReadEntities(cmd);
            cmd.Dispose();
            return results;
        }
        catch (SqlException ex)
        {
            throw new DatabaseException(
                "GetByMeasurementType failed (SQL error " + ex.Number + "): " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("GetByMeasurementType failed: " + ex.Message, ex);
        }
        finally { _pool.ReturnConnection(conn); }
    }

    // ── GetTotalCount ─────────────────────────────────────────────────

    public int GetTotalCount()
    {
        SqlConnection conn = _pool.GetConnection();
        try
        {
            SqlCommand cmd = new SqlCommand("sp_GetTotalCount", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            int count = 0;
            if (reader.Read())
                count = reader.GetInt32(reader.GetOrdinal("TotalCount"));
            reader.Close();
            cmd.Dispose();
            return count;
        }
        catch (SqlException ex)
        {
            throw new DatabaseException(
                "GetTotalCount failed (SQL error " + ex.Number + "): " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("GetTotalCount failed: " + ex.Message, ex);
        }
        finally { _pool.ReturnConnection(conn); }
    }

    // ── DeleteAll ─────────────────────────────────────────────────────

    public void DeleteAll()
    {
        SqlConnection conn = _pool.GetConnection();
        try
        {
            SqlCommand cmd = new SqlCommand("sp_DeleteAllMeasurements", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Console.WriteLine("[DatabaseRepository] All measurements deleted.");
        }
        catch (SqlException ex)
        {
            throw new DatabaseException(
                "DeleteAll failed (SQL error " + ex.Number + "): " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("DeleteAll failed: " + ex.Message, ex);
        }
        finally { _pool.ReturnConnection(conn); }
    }

    // ── Pool / resource ───────────────────────────────────────────────

    public string GetPoolStatistics() => _pool.GetPoolStatistics();

    public void ReleaseResources()
    {
        Console.WriteLine("[DatabaseRepository] Releasing resources...");
        _pool.Dispose();
        Console.WriteLine("[DatabaseRepository] Resources released.");
    }

    // ── Private: map SQL row → YOUR UC15 QuantityMeasurementEntity ────
    // Rebuilds the nested QuantityDTO Operand1/Operand2/Result structure
    // from the flat SQL columns (first_value, first_unit, second_value …)

    private static List<QuantityMeasurementEntity> ReadEntities(SqlCommand cmd)
    {
        var list   = new List<QuantityMeasurementEntity>();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string operation = reader.GetString(reader.GetOrdinal("operation"));
            bool   isError   = reader.GetBoolean(reader.GetOrdinal("is_error"));

            QuantityMeasurementEntity entity;

            if (isError)
            {
                string errMsg = "";
                int errCol = reader.GetOrdinal("error_message");
                if (!reader.IsDBNull(errCol)) errMsg = reader.GetString(errCol);

                // error constructor: (operationType, operand1?, operand2?, errorMessage)
                entity = new QuantityMeasurementEntity(operation, null, null, errMsg);
            }
            else
            {
                string measurementType = "";
                int    mtCol           = reader.GetOrdinal("measurement_type");
                if (!reader.IsDBNull(mtCol)) measurementType = reader.GetString(mtCol);

                double firstValue  = reader.IsDBNull(reader.GetOrdinal("first_value"))  ? 0 : reader.GetDouble(reader.GetOrdinal("first_value"));
                string firstUnit   = reader.IsDBNull(reader.GetOrdinal("first_unit"))   ? "" : reader.GetString(reader.GetOrdinal("first_unit"));
                double secondValue = reader.IsDBNull(reader.GetOrdinal("second_value")) ? 0 : reader.GetDouble(reader.GetOrdinal("second_value"));
                string secondUnit  = reader.IsDBNull(reader.GetOrdinal("second_unit"))  ? "" : reader.GetString(reader.GetOrdinal("second_unit"));
                double resultValue = reader.IsDBNull(reader.GetOrdinal("result_value")) ? 0 : reader.GetDouble(reader.GetOrdinal("result_value"));

                // Rebuild YOUR UC15 QuantityDTO objects
                var operand1 = !string.IsNullOrEmpty(firstUnit)
                    ? new QuantityDTO(firstValue, firstUnit, measurementType)
                    : null;

                var operand2 = !string.IsNullOrEmpty(secondUnit)
                    ? new QuantityDTO(secondValue, secondUnit, measurementType)
                    : null;

                var result = new QuantityDTO(resultValue, "RESULT", measurementType);

                if (operand2 != null)
                    entity = new QuantityMeasurementEntity(operation, operand1!, operand2, result);
                else
                    entity = new QuantityMeasurementEntity(operation, operand1!, result);
            }

            list.Add(entity);
        }

        reader.Close();
        return list;
    }

    // ── Parameter helpers ─────────────────────────────────────────────

    private static void AddParam(SqlCommand cmd, string name, SqlDbType type, int size, object value)
    {
        var p = new SqlParameter(name, type, size);
        p.Value = value;
        cmd.Parameters.Add(p);
    }

    private static void AddParam(SqlCommand cmd, string name, SqlDbType type, object value)
    {
        var p = new SqlParameter(name, type);
        p.Value = value;
        cmd.Parameters.Add(p);
    }
}
