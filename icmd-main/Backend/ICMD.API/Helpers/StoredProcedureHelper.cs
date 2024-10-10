using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.Dtos.Spares;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace ICMD.API.Helpers
{
    public class StoredProcedureHelper
    {
        private readonly IConfiguration _configuration;
        private string connectionString = string.Empty;
        public StoredProcedureHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DBConnectionString") ?? string.Empty;
        }

        //"Server=localhost,5432;Database=Dev_ICMD;User Id=postgres;Password=2015;;Pooling=false;Timeout=300;CommandTimeout=300";
        public async Task<List<AttributeDefinitionIdsDto>> GetAttributeDefinitionIds(string storedProcedureName, NpgsqlParameter[] parameters)
        {
            List<AttributeDefinitionIdsDto> result = new List<AttributeDefinitionIdsDto>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                NpgsqlTransaction tran = connection.BeginTransaction();

                using (var command = new NpgsqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 300;

                    // Add input parameters
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    command.Parameters.Add(new NpgsqlParameter("resultData", NpgsqlTypes.NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = "data1"
                    });

                    command.ExecuteNonQuery();

                    command.CommandText = "fetch all from data1";
                    command.CommandType = CommandType.Text;

                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var dto = new AttributeDefinitionIdsDto()
                            {
                                AttributeDefinitionId = new Guid(row[0]?.ToString() ?? null)
                            };
                            result.Add(dto);
                        }
                    }
                }

                tran.Commit();
                await connection.CloseAsync();
            }

            return result;
        }

        public async Task<bool> ImportOMServiceDescriptions(NpgsqlParameter[] parameters)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlTransaction tran = connection.BeginTransaction();

                    using (var command = new NpgsqlCommand(@"public.""spImportOMServiceDescriptions""", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 300;

                        // Add input parameters
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        command.ExecuteNonQuery();

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);


                        }
                    }

                    tran.Commit();
                    await connection.CloseAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> ImportPNIDTags(NpgsqlParameter[] parameters)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlTransaction tran = connection.BeginTransaction();

                    using (var command = new NpgsqlCommand(@"public.""spImportPnIDTags""", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 300;

                        // Add input parameters
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        command.ExecuteNonQuery();

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                        }
                    }

                    tran.Commit();
                    await connection.CloseAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ViewInstrumentListLiveDto>> GetDuplicateReportsData(string storedProcedureName, Guid? projectId)
        {
            List<ViewInstrumentListLiveDto> result = new List<ViewInstrumentListLiveDto>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                NpgsqlTransaction tran = connection.BeginTransaction();

                using (var command = new NpgsqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 300;

                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter("_ProjectId", NpgsqlDbType.Uuid) { Value = projectId ?? Guid.Empty },
                    };
                    command.Parameters.AddRange(parameters);

                    command.Parameters.Add(new NpgsqlParameter("resultData", NpgsqlTypes.NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = "data1"
                    });

                    command.ExecuteNonQuery();

                    command.CommandText = "fetch all from data1";
                    command.CommandType = CommandType.Text;

                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var dto = new ViewInstrumentListLiveDto()
                            {
                                DeviceId = row["DeviceId"] != DBNull.Value ? new Guid(row["DeviceId"].ToString()) : (Guid?)null,
                                ProcessNo = row["Process No"]?.ToString(),
                                SubProcess = row["Sub Process"]?.ToString(),
                                StreamName = row["StreamName"]?.ToString(),
                                EquipmentCode = row["Equipment Code"]?.ToString(),
                                SequenceNumber = row["Sequence Number"]?.ToString(),
                                EquipmentIdentifier = row["Equipment Identifier"]?.ToString(),
                                TagName = row["TagName"]?.ToString(),
                                InstrumentParentTag = row["Instr Parent Tag"]?.ToString(),
                                ServiceDescription = row["Service Description"]?.ToString(),
                                LineVesselNumber = row["Line / Vessel Number"]?.ToString(),
                                Plant = row["Plant"] != DBNull.Value ? Convert.ToInt32(row["Plant"]) : (int?)null,
                                Area = row["Area"] != DBNull.Value ? Convert.ToInt32(row["Area"]) : (int?)null,
                                VendorSupply = row["Vendor Supply"] != DBNull.Value ? Convert.ToBoolean(row["Vendor Supply"]) : (bool?)null,
                                SkidNumber = row["Skid Number"]?.ToString(),
                                StandNumber = row["Stand Number"]?.ToString(),
                                Manufacturer = row["Manufacturer"]?.ToString(),
                                ModelNumber = row["Model Number"]?.ToString(),
                                CalibratedRangeMin = row["Calibrated Range (Min)"]?.ToString(),
                                CalibratedRangeMax = row["Calibrated Range (Max)"]?.ToString(),
                                CRUnits = row["CR Units"]?.ToString(),
                                ProcessRangeMin = row["Process Range (Min)"]?.ToString(),
                                ProcessRangeMax = row["Process Range (Max)"]?.ToString(),
                                PRUnits = row["PR Units"]?.ToString(),
                                RLPosition = row["RL / Position"]?.ToString(),


                                DatasheetNumber = row["Datasheet Number"]?.ToString(),
                                SheetNumber = row["Sheet Number"]?.ToString(),
                                HookUpDrawing = row["Hook-up Drawing"]?.ToString(),
                                TerminationDiagram = row["Termination Diagram"]?.ToString(),
                                PIDNumber = row["P&Id Number"]?.ToString(),
                                LayoutDrawing = row["Layout Drawing"]?.ToString(),
                                ArchitecturalDrawing = row["Architectural Drawing"]?.ToString(),
                                FunctionalDescriptionDocument = row["Functional Description Document"]?.ToString(),
                                ProductProcurementNumber = row["Product Procurement Number"]?.ToString(),

                                JunctionBoxNumber = row["Junction Box Number"]?.ToString(),
                                NatureOfSignal = row["Nature Of Signal"]?.ToString(),
                                FailState = row["Fail State"]?.ToString(),
                                GSDType = row["GSD Type"]?.ToString(),
                                ControlPanelNumber = row["Control Panel Number"]?.ToString(),
                                PLCNumber = row["PLC Number"]?.ToString(),
                                PLCSlotNumber = row["PLC Slot Number"]?.ToString(),
                                FieldPanelNumber = row["Field Panel Number"]?.ToString(),
                                DPDPCoupler = row["DP/DP Coupler"]?.ToString(),
                                DPPACoupler = row["DP/PA Coupler"]?.ToString(),
                                AFDHubNumber = row["AFD / Hub Number"]?.ToString(),
                                RackNo = row["Rack No"]?.ToString(),
                                SlotNo = row["Slot No"]?.ToString(),

                                ChannelNo = row["Channel No"]?.ToString(),
                                DPNodeAddress = row["DP Node Address"]?.ToString(),
                                PANodeAddress = row["PA Node Address"]?.ToString(),
                                Revision = row["Revision"] != DBNull.Value ? Convert.ToInt32(row["Revision"]) : (int?)null,
                                RevisionChangesOutstandingComments = row["Revision Changes / Outstanding Comments"]?.ToString(),
                                Zone = row["Zone"]?.ToString(),
                                Bank = row["Bank"]?.ToString(),
                                Service = row["Service"]?.ToString(),
                                Variable = row["Variable"]?.ToString(),
                                Train = row["Train"]?.ToString(),
                                WorkAreaPack = row["Work Area Pack"]?.ToString(),
                                SystemCode = row["System Code"]?.ToString(),
                                SubsystemCode = row["SubSystem Code"]?.ToString(),
                                IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : false,
                                IsDeleted = row["IsDeleted"] != DBNull.Value ? Convert.ToBoolean(row["IsDeleted"]) : false,
                            };

                            result.Add(dto);
                        }
                    }
                }

                tran.Commit();
                await connection.CloseAsync();
            }
            return result;
        }

        public async Task<List<SparesReportDto>> GetSparesReportData(string storedProcedureName, Guid? projectId)
        {
            List<SparesReportDto> result = new List<SparesReportDto>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlTransaction tran = connection.BeginTransaction();

                    using (var command = new NpgsqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 300;

                        var parameters = new NpgsqlParameter[]
                        {
                        new NpgsqlParameter("_ProjectId", NpgsqlDbType.Uuid) { Value = projectId ?? Guid.Empty },
                        };
                        command.Parameters.AddRange(parameters);

                        command.Parameters.Add(new NpgsqlParameter("resultData", NpgsqlTypes.NpgsqlDbType.Refcursor)
                        {
                            Direction = ParameterDirection.InputOutput,
                            Value = "data1"
                        });

                        command.ExecuteNonQuery();

                        command.CommandText = "fetch all from data1";
                        command.CommandType = CommandType.Text;

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var dto = new SparesReportDto()
                                {
                                    TotalChanneles = row["Total Channels"] != DBNull.Value ? Convert.ToInt32(row["Total Channels"]) : (int?)null,
                                    UsedChanneles = row["Used Channels"] != DBNull.Value ? Convert.ToInt32(row["Used Channels"]) : 0,
                                    Rack = row["Rack"]?.ToString(),
                                    PLCNumber = row["PLC Number"]?.ToString(),
                                    NatureOfSignal = row["Nature of Signal"]?.ToString(),
                                    SpareChannels = 0,
                                };

                                result.Add(dto);
                            }
                        }
                    }

                    tran.Commit();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public async Task<List<SparesReportDetailsDto>> GetSparesReportDetailsData(string storedProcedureName, Guid? projectId)
        {
            List<SparesReportDetailsDto> result = new List<SparesReportDetailsDto>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlTransaction tran = connection.BeginTransaction();

                    using (var command = new NpgsqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 300;

                        var parameters = new NpgsqlParameter[]
                        {
                        new NpgsqlParameter("_ProjectId", NpgsqlDbType.Uuid) { Value = projectId ?? Guid.Empty },
                        };
                        command.Parameters.AddRange(parameters);

                        command.Parameters.Add(new NpgsqlParameter("resultData", NpgsqlTypes.NpgsqlDbType.Refcursor)
                        {
                            Direction = ParameterDirection.InputOutput,
                            Value = "data1"
                        });

                        command.ExecuteNonQuery();

                        command.CommandText = "fetch all from data1";
                        command.CommandType = CommandType.Text;

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var dto = new SparesReportDetailsDto()
                                {
                                    TotalChanneles = row["Total Channels"] != DBNull.Value ? Convert.ToInt32(row["Total Channels"]) : (int?)null,
                                    UsedChanneles = row["Used Channels"] != DBNull.Value ? Convert.ToInt32(row["Used Channels"]) : 0,
                                    Rack = row["Rack"]?.ToString(),
                                    PLCNumber = row["PLC Number"]?.ToString(),
                                    NatureOfSignal = row["Nature of Signal"]?.ToString(),
                                    SlotNumber = row["Slot Number"] != DBNull.Value ? Convert.ToInt32(row["Slot Number"]) : (int?)null,
                                    SpareChannels = 0,
                                };

                                result.Add(dto);
                            }
                        }
                    }

                    tran.Commit();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public async Task<List<SparesReportPLCDto>> GetSparesReportPLCData(string storedProcedureName, Guid? projectId)
        {
            List<SparesReportPLCDto> result = new List<SparesReportPLCDto>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlTransaction tran = connection.BeginTransaction();

                    using (var command = new NpgsqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 300;

                        var parameters = new NpgsqlParameter[]
                        {
                        new NpgsqlParameter("_ProjectId", NpgsqlDbType.Uuid) { Value = projectId ?? Guid.Empty },
                        };
                        command.Parameters.AddRange(parameters);

                        command.Parameters.Add(new NpgsqlParameter("resultData", NpgsqlTypes.NpgsqlDbType.Refcursor)
                        {
                            Direction = ParameterDirection.InputOutput,
                            Value = "data1"
                        });

                        command.ExecuteNonQuery();

                        command.CommandText = "fetch all from data1";
                        command.CommandType = CommandType.Text;

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var dto = new SparesReportPLCDto()
                                {
                                    TotalChanneles = row["Total Channels"] != DBNull.Value ? Convert.ToInt32(row["Total Channels"]) : (int?)null,
                                    UsedChanneles = row["Used Channels"] != DBNull.Value ? Convert.ToInt32(row["Used Channels"]) : 0,
                                    PLCNumber = row["PLC Number"]?.ToString(),
                                    NatureOfSignal = row["Nature of Signal"]?.ToString(),
                                    SpareChannels = 0,
                                };

                                result.Add(dto);
                            }
                        }
                    }

                    tran.Commit();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }
    }
}
