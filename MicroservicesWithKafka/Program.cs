using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MicroservicesWithKafka.HealthChecks;
using MicroservicesWithKafka.Kafka;
using MicroservicesWithKafka.Middleware;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repositorie;
using MicroservicesWithKafka.Repositories;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Service;
using MicroservicesWithKafka.Services;
using MicroservicesWithKafka.Services.Kafka;
using MicroservicesWithKafka.Services.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Prometheus.DotNetRuntime;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
// Setup Serilog logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
//Add Controllers and Swagger
//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NC API",
        Version = "v1"
    });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token as: Bearer {your token}"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.Configure<NasSettings>(builder.Configuration.GetSection("NasSettings"));
// Configure MongoDB settings from appsettings.json
builder.Services.Configure<FundsDatabaseSettings>(builder.Configuration.GetSection("FundsDatabase"));
// MongoDB Fund Repository and Service DI setup
builder.Services.AddSingleton<IFundRepository, FundRepository>();
builder.Services.AddScoped<FundService>();
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<ICallRegisterRepository, CallRegisterRepository>();
builder.Services.AddScoped<ICallRegisterService, CallRegisterService>();
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddSingleton<KafkaProducerService>();
// test complaint history 
builder.Services.AddSingleton<ComplaintHistorySqlService>();
// SQL Server Service and User Repository setup
builder.Services.AddSingleton<SQLConnectionService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
// category services
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
// BrandMaster services
builder.Services.AddScoped<IBrandMasterRepository, BrandMasterRepository>();
builder.Services.AddScoped<IBrandMasterService, BrandMasterService>();
// ProductMaster services
builder.Services.AddScoped<IProductMasterRepository, ProductMasterRepository>();
builder.Services.AddScoped<IProductMasterService, ProductMasterService>();
// model services
builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<IModelService, ModelService>();
// modelVariant services
builder.Services.AddScoped<IModelVariantRepository, ModelVariantRepository>();
builder.Services.AddScoped<IModelVariantService, ModelVariantService>();
//builder.Services.AddSingleton<SQLConnectionService>();
builder.Services.AddScoped<IMstColorRepository, MstColorRepository>();
builder.Services.AddScoped<IMstColorService, MstColorService>();
// PartMaster services
builder.Services.AddScoped<IPartMasterRepository, PartMasterRepository>();
builder.Services.AddScoped<IPartMasterService, PartMasterService>();
// CityMaster services
builder.Services.AddScoped<ICityMasterRepository, CityMasterRepository>();
builder.Services.AddScoped<ICityMasterService, CityMasterService>();
// RegionMaster services
builder.Services.AddScoped<IRegionMasterRepository, RegionMasterRepository>();
builder.Services.AddScoped<IRegionMasterService, RegionMasterService>();
// StateMaster services
builder.Services.AddScoped<IStateMasterRepository, StateMasterRepository>();
builder.Services.AddScoped<IStateMasterService, StateMasterService>();
// LocalityMaster services
builder.Services.AddScoped<ILocalityMasterRepository, LocalityMasterRepository>();
builder.Services.AddScoped<ILocalityMasterService, LocalityMasterService>();
// RoleMaster services
builder.Services.AddScoped<IRoleMasterRepository, RoleMasterRepository>();
builder.Services.AddScoped<IRoleMasterService, RoleMasterService>();
// DepartmentMaster services
builder.Services.AddScoped<IDepartmentMasterRepository, DepartmentMasterRepository>();
builder.Services.AddScoped<IDepartmentMasterService, DepartmentMasterService>();
// JobService services
builder.Services.AddScoped<IJobServiceRepository, JobServiceRepository>();
builder.Services.AddScoped<IJobServiceService, JobServiceService>();
// SerialNoMaster services
builder.Services.AddScoped<ISerialNoMasterRepository, SerialNoMasterRepository>();
builder.Services.AddScoped<ISerialNoMasterService, SerialNoMasterService>();
// ComplaintHistory services
builder.Services.AddScoped<IComplaintHistoryRepository, ComplaintHistoryRepository>();
builder.Services.AddScoped<IComplaintHistoryService, ComplaintHistoryService>();
// WarrantyRegistration services
builder.Services.AddScoped<IWarrantyRegistrationRepository, WarrantyRegistrationRepository>();
builder.Services.AddScoped<IWarrantyRegistrationService, WarrantyRegistrationService>();
// Register Repository and Service for New Call Register
builder.Services.AddScoped<INewCallRegisterRepository, NewCallRegisterRepository>();
builder.Services.AddScoped<INewCallRegisterService, NewCallRegisterService>();
// CallTypeMaster services
builder.Services.AddScoped<ICallTypeMasterRepository, CallTypeMasterRepository>();
builder.Services.AddScoped<ICallTypeMasterService, CallTypeMasterService>();
// WarrantyMaster services
builder.Services.AddScoped<IWarrantyMasterRepository, WarrantyMasterRepository>();
builder.Services.AddScoped<IWarrantyMasterService, WarrantyMasterService>();
// Register Symptom DI
builder.Services.AddScoped<ISymptomMasterRepository, SymptomMasterRepository>();
builder.Services.AddScoped<ISymptomMasterService, SymptomMasterService>();
// Complaint Cust Detail
builder.Services.AddScoped<IComplaintCustDetailRepository, ComplaintCustDetailRepository>();
builder.Services.AddScoped<IComplaintCustDetailService, ComplaintCustDetailService>();
// Register Services
builder.Services.AddScoped<IPartConsumptionRepository, PartConsumptionRepository>();
builder.Services.AddScoped<IPartConsumptionService, PartConsumptionService>();
// Register your custom services & repositories
builder.Services.AddScoped<ILevelPendingHistoryRepository, LevelPendingHistoryRepository>();
builder.Services.AddScoped<ILevelPendingHistoryService, LevelPendingHistoryService>();
// Register repository + services
builder.Services.AddScoped<IDocumentUploadRepository, DocumentUploadRepository>();
builder.Services.AddScoped<IDocumentUploadService, DocumentUploadService>();
// Register custom services
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();
// Register repository & service
builder.Services.AddScoped<IVisitRepository, VisitRepository>();
builder.Services.AddScoped<IVisitService, VisitService>();
// ASC creation repository and services 
builder.Services.AddScoped<IASCCreationRepository, ASCCreationRepository>();
builder.Services.AddScoped<IASCCreationService, ASCCreationService>();
// Register Repository
builder.Services.AddScoped<IASMMasterRepository, ASMMasterRepository>();
builder.Services.AddScoped<IASMMasterService, ASMMasterService>();
// Call Transfer  Repository
builder.Services.AddScoped<ICallTransferRepository, CallTransferRepository>();
builder.Services.AddScoped<ICallTransferService, CallTransferService>();
// 2. Repository & Service Registration
builder.Services.AddScoped<IDashboardBucketRepository, DashboardBucketRepository>();
builder.Services.AddScoped<IDashboardBucketService, DashboardBucketService>();
// Register your repository & service
builder.Services.AddScoped<IASC_MASTERRepository, ASC_MASTERRepository>();
builder.Services.AddScoped<IASC_MASTERService, ASC_MASTERService>();
// CallTypeRepository your repository & service
builder.Services.AddScoped<ICallTypeRepository, CallTypeRepository>();
builder.Services.AddScoped<ICallTypeService, CallTypeService>();
// for cancellation request
builder.Services.AddScoped<ICancellationService, CancellationService>();
// for JobSheetRepository service
builder.Services.AddScoped<IJobSheetRepository, JobSheetRepository>();
builder.Services.AddScoped<IJobSheetService, JobSheetService>();
// for ClaimProcessRepository service
builder.Services.AddScoped<IClaimProcessRepository, ClaimProcessRepository>();
builder.Services.AddScoped<IClaimProcessService, ClaimProcessService>();
// sync sql to mongo
builder.Services.AddScoped<ISqlToMongoSyncRepository, SqlToMongoSyncRepository>();
builder.Services.AddScoped<ISqlToMongoSyncService, SqlToMongoSyncService>();
// send sms
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRssoftechSMSRepository, RssoftechSMSRepository>();
builder.Services.AddScoped<ISMSService, SMSService>();
// Register Repository
builder.Services.AddScoped<IActionMasterRepository, ActionMasterRepository>();
builder.Services.AddScoped<IActionMasterService, ActionMasterService>();
// Repository & Service DI
builder.Services.AddScoped<ICallCreationUploaderRepository, CallCreationUploaderRepository>();
builder.Services.AddScoped<ICallCreationUploaderService, CallCreationUploaderService>();
// cancellation request new 
builder.Services.AddScoped<IRequestCancellationRepository, RequestCancellationRepository>();
builder.Services.AddScoped<IRequestCancellationService, RequestCancellationService>();
//  dropdownmaster
builder.Services.AddScoped<IDropdownMasterRepository, DropdownMasterRepository>();
builder.Services.AddScoped<IDropdownMasterService, DropdownMasterService>();
//call shedule services
builder.Services.AddScoped<ICallScheduleRepository, CallScheduleRepository>();
builder.Services.AddScoped<ICallScheduleService, CallScheduleService>();
// Happycode generate code
builder.Services.AddScoped<IGenerateHappyCodeRepository, GenerateHappyCodeRepository>();
builder.Services.AddScoped<IGenerateHappyCodeService, GenerateHappyCodeService>();
// CustomerRepository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
// print challan data
builder.Services.AddScoped<IPrintChallanRepository, PrintChallanRepository>();
builder.Services.AddScoped<IPrintChallanService, PrintChallanService>();
// print invoice data
builder.Services.AddScoped<IPrintDataRepository, PrintDataRepository>();
builder.Services.AddScoped<IPrintDataService, PrintDataService>();
// acc for assigned bucket 
builder.Services.AddScoped<ILevel1OperationRepository, Level1OperationRepository>();
builder.Services.AddScoped<ILevel1OperationService, Level1OperationService>();
// check serailno for wip 
builder.Services.AddScoped<ICheckSerialNoRepository, CheckSerialNoRepository>();
builder.Services.AddScoped<ICheckSerialNoService, CheckSerialNoService>();
// po master crud 
builder.Services.AddScoped<IPOMasterRepository, POMasterRepository>();
builder.Services.AddScoped<IPOMasterService, POMasterService>();
// po detail crud 
builder.Services.AddScoped<IPODetailRepository, PODetailRepository>();
builder.Services.AddScoped<IPODetailService, PODetailService>();
// Register Engineer Service & Repository
builder.Services.AddScoped<ISfEngineerRepository, SfEngineerRepository>();
builder.Services.AddScoped<IEngineerService, EngineerService>();
// Register nwh defective Service & Repository
builder.Services.AddScoped<INwhDefectiveRepository, NwhDefectiveRepository>();
builder.Services.AddScoped<INwhDefectiveService, NwhDefectiveService>();
// sf po recive repository
builder.Services.AddScoped<ISFReceivePORepository, SFReceivePORepository>();
builder.Services.AddScoped<ISFReceivePOService, SFReceivePOService>();
// menu master repository
builder.Services.AddScoped<IMenuMasterRepository, MenuMasterRepository>();
builder.Services.AddScoped<IMenuMasterService, MenuMasterService>();
// check happy code repository
builder.Services.AddScoped<ICheckHappyCodeRepository, CheckHappyCodeRepository>();
builder.Services.AddScoped<ICheckHappyCodeService, CheckHappyCodeService>();
// asc pending challan service
builder.Services.AddScoped<IAscPendingChallanRepository, AscPendingChallanRepository>();
builder.Services.AddScoped<IAscPendingChallanService, AscPendingChallanService>();
// asc dispatch challan service
builder.Services.AddScoped<IDispatchLineRepository, DispatchLineRepository>();
builder.Services.AddScoped<IDispatchLineService, DispatchLineService>();
// ascallowance service
builder.Services.AddScoped<ISpecialAllowanceRepository, SpecialAllowanceRepository>();
builder.Services.AddScoped<ISpecialAllowanceService, SpecialAllowanceService>();
// get docket no. service
builder.Services.AddScoped<IDocketRepository, DocketRepository>();
builder.Services.AddScoped<IDocketService, DocketService>();
// get cost service
builder.Services.AddScoped<IGetCostRepository, GetCostRepository>();
builder.Services.AddScoped<IGetCostService, GetCostService>();
// Updated upstream
// process challan service
builder.Services.AddScoped<IChallanRepository, ChallanRepository>();
builder.Services.AddScoped<IChallanService, ChallanService>();
//Stashed changes
//asif
builder.Services.AddScoped<IJobnOtherDetailNewRepository, JobnOtherDetailNewRepository>();
builder.Services.AddScoped<IJobnOtherDetailNewService, JobnOtherDetailNewService>();

// Po validation service
builder.Services.AddScoped<IPOValidationRepository, POValidationRepository>();
builder.Services.AddScoped<IPOValidationService, POValidationService>();

// Po Request service
builder.Services.AddScoped<IPORequestRepository, PORequestRepository>();
builder.Services.AddScoped<IPORequestService, PORequestService>();

// Po Receive service
builder.Services.AddScoped<IPOReceiveRepository, POReceiveRepository>();
builder.Services.AddScoped<IPOReceiveService, POReceiveService>();

// DOA service
builder.Services.AddScoped<IDOARepository, DOARepository>();
builder.Services.AddScoped<IDOAService, DOAService>();

//  Consumption servcie
builder.Services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
builder.Services.AddScoped<IConsumptionService, ConsumptionService>();

//  Consumption servcie
builder.Services.AddScoped<IHappycodeJobCloseCheckRepository, HappycodeJobCloseCheckRepository>();
builder.Services.AddScoped<IHappycodeJobCloseCheckService, HappycodeJobCloseCheckService>();

// sf level1 operation
builder.Services.AddScoped<ISFlevel1OperationRepository, SFlevel1OperationRepository>();
builder.Services.AddScoped<ISFlevel1OperationService, SFlevel1OperationService>();

// gas charging
builder.Services.AddScoped<IGasChargingRepository, GasChargingRepository>();
builder.Services.AddScoped<IGasChargingService, GasChargingService>();

// ok call service
builder.Services.AddScoped<IOKCallRepository, OKCallRepository>();
builder.Services.AddScoped<IOKCallService, OKCallService>();

// stock check by sf service
builder.Services.AddScoped<IStockBySFRepository, StockBySFRepository>();
builder.Services.AddScoped<IStockBySFService, StockBySFService>();

// Symtom Cause analysis service
builder.Services.AddScoped<ISymptomCauseRepository, SymptomCauseRepository>();
builder.Services.AddScoped<ISymptomCauseService,SymptomCauseService>();

// Print job sheet service
builder.Services.AddScoped<IPrintJobsheetRepository, PrintJobsheetRepository>();
builder.Services.AddScoped<IPrintJobsheetService, PrintJobsheetService>();


// new request for cancelation service
builder.Services.AddScoped<ICancellationRequestRepository, CancellationRequestRepository>();
builder.Services.AddScoped<ICancellationRequestService, CancellationRequestService>();

// paid warranty service
builder.Services.AddScoped<IPaidWarrantyRepository, PaidWarrantyRepository>();
builder.Services.AddScoped<IPaidWarrantyService, PaidWarrantyService>();

builder.Services.AddScoped<IClaimProcessRepositoryV1, ClaimProcessRepositoryV1>();
builder.Services.AddScoped<IClaimProcessServiceV1, ClaimProcessServiceV1>();

// finance executve service
builder.Services.AddScoped<IAdminStatusViewRepository, AdminStatusViewRepository>();
builder.Services.AddScoped<IAdminStatusViewService, AdminStatusViewService>();

// munazir  ticket master new development
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();

// TD/DOA CN distributor pickup service
builder.Services.AddScoped<ITDDOAPickupRepository, TDDOAPickupRepository>();
builder.Services.AddScoped<ITDDOAPickupService, TDDOAPickupService>();

// faizan finance claim count service
//builder.Services.AddScoped<IFinanceClaimCountRepository, FinanceClaimCountRepository>();
//builder.Services.AddScoped<IFinanceClaimCountService, FinanceClaimCountService>();


// healt check 
builder.Services.AddHealthChecks()
    .AddCheck<HostingHealthCheck>("hosting")
    .AddCheck<KafkaReplicaHealthCheck>("kafka")
    .AddCheck<MongoReplicaHealthCheck>("mongo-replica")
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),name: "sql");

// JWT Authentication Setup
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Custom error handling events
    options.Events = new JwtBearerEvents
    {
        //new add for
        OnMessageReceived = context =>
        {
            // IMPORTANT: Do NOT validate token for login API
            if (context.HttpContext.Request.Path.StartsWithSegments("/api/users/login"))
            {
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        },

        //end 
        // Triggered when token validation fails (expired/invalid)
        OnAuthenticationFailed = context =>
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                error = "Authentication failed",
                detail = context.Exception.Message
            });
            return context.Response.WriteAsync(result);
        },

        // Triggered when user is not logged in or token missing
        OnChallenge = context =>
        {
            context.HandleResponse(); // Prevent default 401 HTML page
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                error = "Unauthorized",
                message = "You must be logged in to access this resource."
            });
            return context.Response.WriteAsync(result);
        },

        // Triggered when user has valid token but not enough permissions (403)
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                error = "Forbidden",
                message = "You do not have permission to access this resource."
            });
            return context.Response.WriteAsync(result);
        },

        // new add for validation for role Triggered when the token is successfully validated
        OnTokenValidated = context =>
        {
            // Extract the "rolename" claim from the token
            var roleClaim = context.Principal?.FindFirst("rolename");

            if (roleClaim != null)
            {
                // If "rolename" exists, add it to ClaimTypes.Role
                var identity = context.Principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));  // Map rolename to ClaimTypes.Role
                }
            }

            return Task.CompletedTask;
        }
    };

});

// 4. Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // COMMON POLICY (for all roles - allows access to general APIs)
    options.AddPolicy("CommonUserPolicy", policy =>
        policy.RequireRole(
            "HO", "BR", "SF/SSD", "CC", "HOuser", "ASM", "ZONE", "BSM", "Customer",
            "Logistic", "NWH", "GSTTeam", "ECom", "HOuser1", "Logistic1", "Reliance",
            "Engineer", "TechnicalExe", "NWHExe", "ASCM", "ASMM", "NWHM", "HOM",
            "FINANCEM", "Finance"
        ));  
    // INDIVIDUAL ROLE POLICIES (for restricted access)
    options.AddPolicy("SFSSDPolicy", policy =>
        policy.RequireRole("SF/SSD"));  // SF/SSD role policy
    // Head Office
    options.AddPolicy("HOUserPolicy", policy =>
        policy.RequireRole("HO", "HOuser", "HOuser1", "HOM"));
    // Finance Team
    options.AddPolicy("FinancePolicy", policy =>
        policy.RequireRole("FINANCEM", "Finance", "GSTTeam"));
    // Branch Team
    options.AddPolicy("BranchPolicy", policy =>
        policy.RequireRole("BR", "BSM"));
    // Service Field / Engineers
    options.AddPolicy("ServiceFieldPolicy", policy =>
        policy.RequireRole("SF/SSD", "Engineer", "TechnicalExe"));
    // ASM Group
    options.AddPolicy("ASMPolicy", policy =>
        policy.RequireRole("ASM", "ASCM", "ASMM"));
    // Logistic Group
    options.AddPolicy("LogisticPolicy", policy =>
        policy.RequireRole("Logistic", "Logistic1", "NWH", "NWHExe", "NWHM"));
    // Customer Group
    options.AddPolicy("CustomerPolicy", policy =>
        policy.RequireRole("Customer", "Reliance", "ECom"));
    // Zone Group
    options.AddPolicy("ZonePolicy", policy =>
        policy.RequireRole("ZONE"));
    // Call Center
    options.AddPolicy("CallCenterPolicy", policy =>
        policy.RequireRole("CC"));
    // Admin / Super Access
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("HO", "HOuser", "HOM", "FINANCEM", "Finance"));
});
// Kafka Producer setup
builder.Services.AddSingleton<KafkaProducer>(provider =>
    new KafkaProducer(builder.Configuration.GetSection("Kafka:BootstrapServers").Value));
builder.Services.AddSingleton<BaseService>();
builder.Services.AddSingleton<IServiceFactory, ServiceFactory>();
builder.Services.AddSingleton<IBaseService<Fund>, FundService>();
// Encryption Service (used in custom middleware)
builder.Services.AddSingleton<IEncryptionService>(provider =>
{
    string encryptionKey = builder.Configuration.GetSection("Middleware:encryptionKey").Value;
    string encryptionIv = builder.Configuration.GetSection("Middleware:encryptionIv").Value;
    return new EncryptionService(encryptionKey, encryptionIv);
});
// Optional: Enable CORS (required if API accessed externally via frontend/Postman)
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//    {
//        builder.AllowAnyOrigin()
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("https://crmnew.jainaindia.com", "https://crmnra.jainaindia.com", "https://devcrmfrontend.jainaindia.com", "https://crmcore.jainaindia.com", "https://dev-crmapp.jainaindia.com") // your frontend URLs
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // safe because we specify origin  http://localhost:3000 -- https://crmcore.jainaindia.com
    });
});
var app = builder.Build();
// Collect .NET runtime metrics (GC, exceptions, threads, etc.)
DotNetRuntimeStatsBuilder.Default().StartCollecting();
// Middleware for Prometheus metrics
app.UseHttpMetrics(); // adds HTTP request duration metrics
app.MapGet("/", () => "Hello from .NET 8 + Prometheus!");
// Metrics endpoint
app.MapMetrics("/metrics");
// Global error handler for capturing unhandled exceptions (good for IIS visibility)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        Log.Error(exceptionHandlerPathFeature?.Error, "Unhandled exception");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Internal Server Error",
            detail = exceptionHandlerPathFeature?.Error?.Message
        });
    });
});
// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable Serilog HTTP request logging
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);

        // Not adding headers/body/tokens here
    };
});

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();
// Routing middleware is required before Authorization and endpoint mapping
app.UseRouting();


// HEALTH CHECK ENDPOINTS

// Used by Load Balancer / Kubernetes to check if app is UP
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Name == "hosting"
});

// Checks external dependencies (Kafka, Mongo, SQL)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check =>
        check.Name == "kafka" ||
        check.Name == "mongo-replica" ||
        check.Name == "sql",

    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                service = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };

        await context.Response.WriteAsJsonAsync(response);
    }
});

// Detailed health (for DevOps / debugging)
app.MapHealthChecks("/health/details", new HealthCheckOptions
{
    Predicate = _ => true,

    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            dependencies = report.Entries.ToDictionary(
                e => e.Key,
                e => e.Value.Status.ToString()
            )
        };

        await context.Response.WriteAsJsonAsync(response);
    }
});


app.UseCors("AllowFrontend");
// ADD after UseRouting()
app.UseAuthentication(); // JWT Authentication
app.UseAuthorization();
//app.UseCors("AllowFrontend");
// Custom Middleware (must be after routing if accessing route data)
app.UseQueryDecryption();
// Map controller endpoints
app.MapControllers();
// Run the app
app.Run();
