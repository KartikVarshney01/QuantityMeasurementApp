using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementAppBusinessLayer.Interfaces;
using QuantityMeasurementAppModelLayer.DTOs;

namespace QuantityMeasurementAPI.Controllers;

/// <summary>
/// Exposes all Quantity Measurement operations as RESTful HTTP endpoints.
/// <para>
/// All request bodies are validated automatically via DataAnnotation attributes
/// on the DTO classes in <c>QuantityMeasurementAppModelLayer.DTOs</c>.
/// Model validation errors are returned as HTTP 400 with full field-level details
/// before any service logic executes.
/// </para>
/// <para>
/// Unhandled exceptions are caught by <c>GlobalExceptionHandlingMiddleware</c>
/// and converted to structured <see cref="ErrorResponse"/> JSON payloads.
/// </para>
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
// This is our main controller that handles all the web requests.
// ASP.NET Core does a lot of work for us here, like checking if the data 
// sent by the user is correctly formatted before our code even starts.
public class QuantitiesController : ControllerBase
{
    private readonly IQuantityMeasurementService   _service;
    private readonly ILogger<QuantitiesController> _logger;

    /// <summary>
    /// Initialises the controller with the service and logger via ASP.NET Core DI.
    /// </summary>
    /// <param name="service">The quantity measurement service.</param>
    /// <param name="logger">The structured logger.</param>
    public QuantitiesController(
        IQuantityMeasurementService service,
        ILogger<QuantitiesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger  = logger  ?? throw new ArgumentNullException(nameof(logger));
    }

    // ── POST /api/quantities/compare ──────────────────────────────────

    /// <summary>
    /// Compares two quantities of the same measurement category.
    /// Both values are first converted to their category's base unit before comparison,
    /// so <c>1 Feet</c> and <c>12 Inches</c> are considered equal.
    /// </summary>
    /// <param name="request">
    /// A <see cref="BinaryOperationRequest"/> containing two quantities (Q1 and Q2).
    /// Both must share the same <c>Category</c> (LENGTH, WEIGHT, VOLUME, or TEMPERATURE).
    /// </param>
    /// <returns>
    /// <see cref="ComparisonResponse"/> wrapped in <see cref="ApiResponse{T}"/>
    /// with <c>AreEqual</c> set to <c>true</c> or <c>false</c>.
    /// </returns>
    /// <response code="200">Comparison performed successfully.</response>
    /// <response code="400">
    /// Returned when:
    /// <list type="bullet">
    ///   <item>Request body fails model validation (missing fields, invalid category).</item>
    ///   <item>Q1 and Q2 have different categories.</item>
    ///   <item>A unit name is not recognised.</item>
    /// </list>
    /// </response>
    /// <response code="500">Unexpected server error.</response>
    [HttpPost("compare")]
    [ProducesResponseType(typeof(ApiResponse<ComparisonResponse>),   StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                      StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                      StatusCodes.Status500InternalServerError)]
    public IActionResult Compare([FromBody] BinaryOperationRequest request)
    {
        _logger.LogInformation("Compare called — Q1: {Q1}, Q2: {Q2}", request.Q1, request.Q2);

        bool equal = _service.Compare(MapToDTO(request.Q1), MapToDTO(request.Q2));

        var payload = new ComparisonResponse
        {
            AreEqual = equal,
            Message  = equal ? "Quantities are equal." : "Quantities are not equal."
        };

        return Ok(new ApiResponse<ComparisonResponse>(true, "Comparison successful.", payload));
    }

    // ── POST /api/quantities/convert ──────────────────────────────────

    /// <summary>
    /// Converts a quantity from its current unit to the specified target unit.
    /// Both units must belong to the same measurement category as the source quantity.
    /// </summary>
    /// <param name="request">
    /// A <see cref="ConversionRequest"/> containing the source quantity and the
    /// <c>TargetUnit</c> name (e.g. "Inch", "Gram", "Fahrenheit").
    /// </param>
    /// <returns>
    /// <see cref="ConversionResponse"/> containing the converted numeric value,
    /// unit name, and category.
    /// </returns>
    /// <response code="200">Conversion performed successfully.</response>
    /// <response code="400">
    /// Returned when:
    /// <list type="bullet">
    ///   <item>Request body fails model validation.</item>
    ///   <item>The source or target unit is not recognised.</item>
    ///   <item>The target unit belongs to a different category.</item>
    /// </list>
    /// </response>
    /// <response code="500">Unexpected server error.</response>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ApiResponse<ConversionResponse>),    StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                       StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                       StatusCodes.Status500InternalServerError)]
    public IActionResult Convert([FromBody] ConversionRequest request)
    {
        _logger.LogInformation("Convert called — {Qty} → {Target}",
            request.Quantity, request.TargetUnit);

        var result = _service.Convert(MapToDTO(request.Quantity), request.TargetUnit);

        var payload = new ConversionResponse
        {
            Result   = result.Value,
            Unit     = result.UnitName,
            Category = result.Category
        };

        return Ok(new ApiResponse<ConversionResponse>(true, "Conversion successful.", payload));
    }

    // ── POST /api/quantities/add ──────────────────────────────────────

    /// <summary>
    /// Adds two quantities of the same measurement category.
    /// Q2 is first converted to Q1's base unit before addition.
    /// The result is expressed in Q1's unit.
    /// Temperature addition is not supported.
    /// </summary>
    /// <param name="request">
    /// A <see cref="BinaryOperationRequest"/> where Q1 and Q2 share the same category.
    /// </param>
    /// <returns>
    /// <see cref="ArithmeticOperationResponse"/> with the sum expressed in Q1's unit.
    /// </returns>
    /// <response code="200">Addition performed successfully.</response>
    /// <response code="400">
    /// Returned when:
    /// <list type="bullet">
    ///   <item>Request body fails model validation.</item>
    ///   <item>Q1 and Q2 have different categories.</item>
    ///   <item>Category is TEMPERATURE (arithmetic not supported).</item>
    /// </list>
    /// </response>
    /// <response code="500">Unexpected server error.</response>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<ArithmeticOperationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                             StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                             StatusCodes.Status500InternalServerError)]
    public IActionResult Add([FromBody] BinaryOperationRequest request)
    {
        _logger.LogInformation("Add called — Q1: {Q1}, Q2: {Q2}", request.Q1, request.Q2);

        var result = _service.Add(MapToDTO(request.Q1), MapToDTO(request.Q2));

        var payload = new ArithmeticOperationResponse
        {
            Result   = result.Value,
            Unit     = result.UnitName,
            Category = result.Category
        };

        return Ok(new ApiResponse<ArithmeticOperationResponse>(true, "Addition successful.", payload));
    }

    // ── POST /api/quantities/subtract ─────────────────────────────────

    /// <summary>
    /// Subtracts Q2 from Q1. Both must share the same measurement category.
    /// Q2 is first converted to Q1's base unit before subtraction.
    /// The result is expressed in Q1's unit.
    /// Temperature subtraction is not supported.
    /// </summary>
    /// <param name="request">
    /// A <see cref="BinaryOperationRequest"/> where Q1 and Q2 share the same category.
    /// </param>
    /// <returns>
    /// <see cref="ArithmeticOperationResponse"/> with the difference expressed in Q1's unit.
    /// </returns>
    /// <response code="200">Subtraction performed successfully.</response>
    /// <response code="400">
    /// Returned when:
    /// <list type="bullet">
    ///   <item>Request body fails model validation.</item>
    ///   <item>Q1 and Q2 have different categories.</item>
    ///   <item>Category is TEMPERATURE (arithmetic not supported).</item>
    /// </list>
    /// </response>
    /// <response code="500">Unexpected server error.</response>
    [HttpPost("subtract")]
    [ProducesResponseType(typeof(ApiResponse<ArithmeticOperationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                             StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                             StatusCodes.Status500InternalServerError)]
    public IActionResult Subtract([FromBody] BinaryOperationRequest request)
    {
        _logger.LogInformation("Subtract called — Q1: {Q1}, Q2: {Q2}", request.Q1, request.Q2);

        var result = _service.Subtract(MapToDTO(request.Q1), MapToDTO(request.Q2));

        var payload = new ArithmeticOperationResponse
        {
            Result   = result.Value,
            Unit     = result.UnitName,
            Category = result.Category
        };

        return Ok(new ApiResponse<ArithmeticOperationResponse>(
            true, "Subtraction successful.", payload));
    }

    // ── POST /api/quantities/divide ───────────────────────────────────

    /// <summary>
    /// Divides Q1 by Q2. Both must share the same measurement category.
    /// Both values are converted to base units before division.
    /// The result is a dimensionless scalar ratio (no unit).
    /// Temperature division is not supported.
    /// </summary>
    /// <param name="request">
    /// A <see cref="BinaryOperationRequest"/> where Q1 and Q2 share the same category.
    /// Q2's base-unit value must not be zero.
    /// </param>
    /// <returns>
    /// <see cref="DivisionResponse"/> containing the scalar ratio.
    /// </returns>
    /// <response code="200">Division performed successfully.</response>
    /// <response code="400">
    /// Returned when:
    /// <list type="bullet">
    ///   <item>Request body fails model validation.</item>
    ///   <item>Q1 and Q2 have different categories.</item>
    ///   <item>Category is TEMPERATURE (arithmetic not supported).</item>
    ///   <item>Q2 evaluates to zero in base units (division by zero).</item>
    /// </list>
    /// </response>
    /// <response code="500">Unexpected server error.</response>
    [HttpPost("divide")]
    [ProducesResponseType(typeof(ApiResponse<DivisionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                  StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                  StatusCodes.Status500InternalServerError)]
    public IActionResult Divide([FromBody] BinaryOperationRequest request)
    {
        _logger.LogInformation("Divide called — Q1: {Q1}, Q2: {Q2}", request.Q1, request.Q2);

        double result = _service.Divide(MapToDTO(request.Q1), MapToDTO(request.Q2));

        return Ok(new ApiResponse<DivisionResponse>(
            true, "Division successful.", new DivisionResponse { Result = result }));
    }

    // ── GET /api/quantities/history ───────────────────────────────────

    /// <summary>
    /// Retrieves the full history of all quantity measurement operations,
    /// ordered by most recent first.
    /// </summary>
    /// <returns>
    /// A list of <see cref="OperationHistoryResponse"/> objects wrapped in
    /// <see cref="ApiResponse{T}"/>. Returns an empty list when no operations have been recorded.
    /// </returns>
    /// <response code="200">History retrieved successfully.</response>
    /// <response code="500">Unexpected server error.</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<List<OperationHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                                StatusCodes.Status500InternalServerError)]
    public IActionResult GetHistory()
    {
        _logger.LogInformation("GetHistory called.");
        var history = MapHistory(_service.GetHistory());
        return Ok(new ApiResponse<List<OperationHistoryResponse>>(
            true, "History retrieved successfully.", history));
    }

    // ── GET /api/quantities/history/operation/{operation} ─────────────

    /// <summary>
    /// Retrieves history filtered by operation type, ordered by most recent first.
    /// </summary>
    /// <param name="operation">
    /// Operation name to filter by. Valid values: <c>Compare</c>, <c>Convert</c>,
    /// <c>Add</c>, <c>Subtract</c>, <c>Divide</c> (case-insensitive).
    /// </param>
    /// <returns>
    /// A filtered list of <see cref="OperationHistoryResponse"/> objects.
    /// Returns an empty list when no matching records exist.
    /// </returns>
    /// <response code="200">History retrieved successfully.</response>
    /// <response code="400">Returned when <paramref name="operation"/> is null or empty.</response>
    /// <response code="500">Unexpected server error.</response>
    [HttpGet("history/operation/{operation}")]
    [ProducesResponseType(typeof(ApiResponse<List<OperationHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                                StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                                StatusCodes.Status500InternalServerError)]
    public IActionResult GetByOperation(string operation)
    {
        _logger.LogInformation("GetByOperation called — operation: {Operation}", operation);
        var history = MapHistory(_service.GetByOperation(operation));
        return Ok(new ApiResponse<List<OperationHistoryResponse>>(
            true, $"History for operation '{operation}' retrieved.", history));
    }

    // ── GET /api/quantities/history/type/{measurementType} ────────────

    /// <summary>
    /// Retrieves history filtered by measurement category, ordered by most recent first.
    /// </summary>
    /// <param name="measurementType">
    /// Category to filter by. Valid values: <c>LENGTH</c>, <c>WEIGHT</c>,
    /// <c>VOLUME</c>, <c>TEMPERATURE</c> (case-insensitive).
    /// </param>
    /// <returns>
    /// A filtered list of <see cref="OperationHistoryResponse"/> objects.
    /// Returns an empty list when no matching records exist.
    /// </returns>
    /// <response code="200">History retrieved successfully.</response>
    /// <response code="400">Returned when <paramref name="measurementType"/> is null or empty.</response>
    /// <response code="500">Unexpected server error.</response>
    [HttpGet("history/type/{measurementType}")]
    [ProducesResponseType(typeof(ApiResponse<List<OperationHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),                                StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse),                                StatusCodes.Status500InternalServerError)]
    public IActionResult GetByMeasurementType(string measurementType)
    {
        _logger.LogInformation("GetByMeasurementType called — type: {Type}", measurementType);
        var history = MapHistory(_service.GetByMeasurementType(measurementType));
        return Ok(new ApiResponse<List<OperationHistoryResponse>>(
            true, $"History for type '{measurementType}' retrieved.", history));
    }

    // ── GET /api/quantities/count ─────────────────────────────────────

    /// <summary>
    /// Returns the total number of measurement operations persisted so far.
    /// Includes both successful and error records.
    /// </summary>
    /// <returns>
    /// <see cref="CountResponse"/> with <c>TotalOperations</c> set to the record count.
    /// </returns>
    /// <response code="200">Count retrieved successfully.</response>
    /// <response code="500">Unexpected server error.</response>
    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<CountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse),               StatusCodes.Status500InternalServerError)]
    public IActionResult GetCount()
    {
        _logger.LogInformation("GetCount called.");
        int count = _service.GetCount();
        return Ok(new ApiResponse<CountResponse>(
            true, "Count retrieved successfully.",
            new CountResponse { TotalOperations = count }));
    }

    // ── GET /api/quantities/health ────────────────────────────────────

    /// <summary>
    /// Simple health-check endpoint.
    /// Returns HTTP 200 with a status message and current UTC timestamp.
    /// Use this to verify the API is running before sending real requests.
    /// </summary>
    /// <returns>An anonymous object with <c>status</c> and <c>timestamp</c> fields.</returns>
    /// <response code="200">API is running and healthy.</response>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        _logger.LogInformation("Health check called.");
        return Ok(new { status = "Quantity Measurement API is running.", timestamp = DateTime.UtcNow });
    }

    // ── Private helpers ───────────────────────────────────────────────

    /// <summary>Maps a <see cref="QuantityRequest"/> to a <see cref="QuantityDTO"/>.</summary>
    private static QuantityDTO MapToDTO(QuantityRequest r)
        => new(r.Value, r.UnitName, r.Category);

    /// <summary>Projects a list of entities to <see cref="OperationHistoryResponse"/> objects.</summary>
    private static List<OperationHistoryResponse> MapHistory(
        List<QuantityMeasurementAppModelLayer.Entities.QuantityMeasurementEntity> entities)
        => entities.Select(h => new OperationHistoryResponse
        {
            Id           = h.Id,
            Operation    = h.Operation,
            Operand1     = h.Operand1,
            Operand2     = h.Operand2,
            Result       = h.Result,
            HasError     = h.HasError,
            ErrorMessage = h.ErrorMessage,
            CreatedAt    = h.CreatedAt
        }).ToList();
}
