using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Services.Sisco;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Sisco.Controller
{
    [Area("Panel")]
    [Authorize]
    public class SiscoManager : NikController
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSysBaseServ;
        private readonly ISiscoService _iSiscoServ;

        public SiscoManager(
            IConfiguration config,
            IWebHostEnvironment hostingEnv,
            UserManager<User> userManager,
             ISystemBaseService iSysBaseServ,
             ISiscoService iSiscoServ
            )
        {
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _config = config;
            _iSysBaseServ = iSysBaseServ;
            _iSiscoServ = iSiscoServ;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Messages = Messages;
            var records = new List<SiscoBaseModel>();
            ViewBag.Records = records;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CsvDataRequest request)
        {
            var user = _userManager.GetUserAsync(HttpContext.User);
            var records = new List<SiscoBaseModel>();
            ViewBag.Messages = Messages;

            if (request.Source == null || request.Source.Length == 0)
            {
                ViewBag.Records = records;
                AddError("هیچ فایلی انتخاب نشده است");
                ViewBag.Messages = Messages;
                return View(request);
            }

            if (request.SubmitValue == "import")
                await ImportData(request);

            using var memoryStream = new MemoryStream(new byte[request.Source.Length]);
            await request.Source.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var settings = await _iSysBaseServ.iBaseInfoServ.FindAsync(x => x.KeyValue == "PerFix" && x.UserId == user.Id);

            using (var reader = new StreamReader(memoryStream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = csv.GetRecord<SiscoBaseModel>();
                        if (request.SubmitValue != "json")
                        {

                            string normalNum = string.Empty;
                            if (settings != null)
                            {
                                normalNum = _iSiscoServ.GetNormalPartyNumber(record.callingPartyNumber, settings.StringValue);
                                if (settings.BoolValue)
                                {
                                    if (record.finalCalledPartyNumber.StartsWith(settings.StringValue))
                                    {
                                        record.finalCalledPartyNumber = record.finalCalledPartyNumber.Substring(settings.StringValue.Length);
                                    }
                                }
                            }
                            else
                            {
                                normalNum = _iSiscoServ.GetNormalPartyNumber(record.callingPartyNumber);
                            }
                            //Normalizing Step 1
                            if ((record.incomingProtocolID != 3 || record.outgoingProtocolID != 3 || record.origCause_value != "393216" || record.destCause_value != "393216") && !string.IsNullOrEmpty(record.callingPartyNumber)
                                && !record.callingPartyNumber.StartsWith("b00") && normalNum != null)
                            {
                                record.callingPartyNumber = normalNum;
                                records.Add(record);
                            }
                        }
                        else
                        {
                            records.Add(record);
                        }
                    }
                }
            }

            if (request.SubmitValue == "json")
                return Ok(records);

            //var fileextension = Path.GetExtension(request.Source.FileName);
            //var filename = Guid.NewGuid().ToString() + fileextension;
            //var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/Sisco", filename);
            //using (FileStream fs = System.IO.File.Create(filepath))
            //{
            //    request.Source.CopyTo(fs);
            //}

            //var records = new List<SiscoBaseModel>();
            //if (fileextension == ".csv")
            //{
            //    using (var reader = new StreamReader(filepath))
            //    {
            //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //        {
            //            records = csv.GetRecords<SiscoBaseModel>().ToList();
            //        }
            //    }
            //}

            ViewBag.Records = records;

            return View();
        }

        public async Task<bool> ImportData(CsvDataRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            using var memoryStream = new MemoryStream(new byte[request.Source.Length]);
            await request.Source.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using (var reader = new StreamReader(memoryStream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = csv.GetRecord<SiscoBaseModel>();
                        var normalNum = _iSiscoServ.GetNormalPartyNumber(record.callingPartyNumber);
                        //Normalizing Step 1
                        if ((record.incomingProtocolID != 3 || record.outgoingProtocolID != 3 || record.origCause_value != "393216" || record.destCause_value != "393216") && !string.IsNullOrEmpty(record.callingPartyNumber)
                            && !record.callingPartyNumber.StartsWith("b00") && normalNum != null)
                        {
                            var item = new SiscoRecord();
                            item.UserId = user.Id;
                            item.CDRRecordType = record.cdrRecordType;
                            item.GCIDCallManagerId = record.globalCallID_callManagerId;
                            item.GCIDCallId = record.globalCallID_callId;
                            item.OrigLegCallIdentifier = record.origLegCallIdentifier;
                            item.DateTimeOrigination = record.dateTimeOrigination;
                            item.OrigNodeId = record.origNodeId;
                            item.OrigSpan = record.origSpan;
                            item.OrigIpAddr = record.origIpAddr;
                            item.CPartyNumber = normalNum;
                            item.CPartyUCLoginUserID = record.callingPartyUnicodeLoginUserID;
                            item.OrigCauseLocation = record.origCause_location;
                            item.OrigCauseValue = record.origCause_value;
                            item.OrigPrecedenceLevel = record.origPrecedenceLevel;
                            item.OrigMediaTransportAIP = record.origMediaTransportAddress_IP;
                            item.OrigMediaTransportAPort = record.origMediaTransportAddress_Port;
                            item.OrigMediaCapPayloadCapability = record.origMediaCap_payloadCapability;
                            item.OrigMediaCapMaxFramesPerPacket = record.origMediaCap_maxFramesPerPacket;
                            item.OrigMediaCapG723BitRate = record.origMediaCap_g723BitRate;
                            item.OrigVideoCapCodec = record.origVideoCap_Codec;
                            item.OrigVideoCapBandwidth = record.origVideoCap_Bandwidth;
                            item.OrigVideoCapResolution = record.origVideoCap_Resolution;
                            item.OrigVideoTransportAIP = record.origVideoTransportAddress_IP;
                            item.OrigVideoTransportAPort = record.origVideoTransportAddress_Port;
                            item.OrigRSVPAudioStat = record.origRSVPAudioStat;
                            item.OrigRSVPVideoStat = record.origRSVPVideoStat;
                            item.DestLegIdentifier = record.destLegIdentifier;
                            item.DestNodeId = record.destNodeId;
                            item.DestSpan = record.destSpan;
                            item.DestIpAddr = record.destIpAddr;
                            item.OriginalCalledPartyNumber = record.originalCalledPartyNumber;
                            item.FinalCalledPartyNumber = record.finalCalledPartyNumber;
                            item.FinalCalledPartyUnicodeLUID = record.finalCalledPartyUnicodeLoginUserID;
                            item.DestCauseLocation = record.destCause_location;
                            item.DestCauseValue = record.destCause_value;
                            item.DestPrecedenceLevel = record.destPrecedenceLevel;
                            item.DestMediaTransportAIP = record.destMediaTransportAddress_IP;
                            item.DestMediaTransportAPort = record.destMediaTransportAddress_Port;
                            item.DestMediaCapPayloadCapability = record.destMediaCap_payloadCapability;
                            item.DestMediaCapMaxFramesPerPacket = record.destMediaCap_maxFramesPerPacket;
                            item.DestMediaCapG723BitRate = record.destMediaCap_g723BitRate;
                            item.DestVideoCapCodec = record.destVideoCap_Codec;
                            item.DestVideoCapBandwidth = record.destVideoCap_Bandwidth;
                            item.DestVideoCapResolution = record.destVideoCap_Resolution;
                            item.DestVideoTransportAIP = record.destVideoTransportAddress_IP;
                            item.DestVideoTransportAPort = record.destVideoTransportAddress_Port;
                            item.DestRSVPAudioStat = record.destRSVPAudioStat;
                            item.DestRSVPVideoStat = record.destRSVPVideoStat;
                            item.DateTimeConnect = record.dateTimeConnect;
                            item.DateTimeDisconnect = record.dateTimeDisconnect;
                            item.LastRedirectDn = record.lastRedirectDn;
                            item.Pkid = record.pkid;
                            item.OrigCalledPartyNumPartition = record.originalCalledPartyNumberPartition;
                            item.CallingPartyNumPartition = record.callingPartyNumberPartition;
                            item.FinalCalledPartyNumPartition = record.finalCalledPartyNumberPartition;
                            item.LastRedirectDnPartition = record.lastRedirectDnPartition;
                            item.Duration = record.duration;
                            item.OrigDeviceName = record.origDeviceName;
                            item.DestDeviceName = record.destDeviceName;
                            item.OrigCallTerminOnBehalfOf = record.origCallTerminationOnBehalfOf;
                            item.DestCallTerminOnBehalfOf = record.destCallTerminationOnBehalfOf;
                            item.OrigCallPartyRedirOnBehalfOf = record.origCalledPartyRedirectOnBehalfOf;
                            item.LastRedirOnBehalfOf = record.lastRedirectRedirectOnBehalfOf;
                            item.OrigCalledPartyRedirReason = record.origCalledPartyRedirectReason;
                            item.LastRedirReason = record.lastRedirectRedirectReason;
                            item.DestConversationId = record.destConversationId;
                            item.GlobalCallIdClusterID = record.globalCallId_ClusterID;
                            item.JoinOnBehalfOf = record.joinOnBehalfOf;
                            item.Comment = record.comment;
                            item.AuthCodeDescription = record.authCodeDescription;
                            item.AuthorizationLevel = record.authorizationLevel;
                            item.ClientMatterCode = record.clientMatterCode;
                            item.OrigDTMFMethod = record.origDTMFMethod;
                            item.DestDTMFMethod = record.destDTMFMethod;
                            item.CallSecuredStatus = record.callSecuredStatus;
                            item.OrigConversationId = record.origConversationId;
                            item.OrigMediaCapBandwidth = record.origMediaCap_Bandwidth;
                            item.DestMediaCapBandwidth = record.destMediaCap_Bandwidth;
                            item.AuthorizationCodeValue = record.authorizationCodeValue;
                            item.OutpulsedCallingPartyNum = record.outpulsedCallingPartyNumber;
                            item.OutpulsedCalledPartyNum = record.outpulsedCalledPartyNumber;
                            item.OrigIpv4v6Addr = record.origIpv4v6Addr;
                            item.DestIpv4v6Addr = record.destIpv4v6Addr;
                            item.OrigVideoCapCodecChl2 = record.origVideoCap_Codec_Channel2;
                            item.OrigVideoCapBandwidthChl2 = record.origVideoCap_Bandwidth_Channel2;
                            item.OrigVideoCapResolutionChl2 = record.origVideoCap_Resolution_Channel2;
                            item.OrigVideoTransportAIPChl2 = record.origVideoTransportAddress_IP_Channel2;
                            item.OrigVideoTransportAPortChl2 = record.origVideoTransportAddress_Port_Channel2;
                            item.OrigVideoChannelRoleChl2 = record.origVideoChannel_Role_Channel2;
                            item.DestVideoCapCodecChl2 = record.destVideoCap_Codec_Channel2;
                            item.DestVideoCapBandwidthChl2 = record.destVideoCap_Bandwidth_Channel2;
                            item.DestVideoCapResolutionChl2 = record.destVideoCap_Resolution_Channel2;
                            item.DestVideoTransportAIPChl2 = record.destVideoTransportAddress_IP_Channel2;
                            item.DestVideoTransportAPortChl2 = record.destVideoTransportAddress_Port_Channel2;
                            item.DestVideoChannelRoleChl2 = record.destVideoChannel_Role_Channel2;
                            item.IncomingProtocolID = record.incomingProtocolID;
                            item.IncomingProtocolCallRef = record.incomingProtocolCallRef;
                            item.OutgoingProtocolID = record.outgoingProtocolID;
                            item.OutgoingProtocolCallRef = record.outgoingProtocolCallRef;
                            item.CurrentRoutingReason = record.currentRoutingReason;
                            item.OrigRoutingReason = record.origRoutingReason;
                            item.LastRedirectingRoutingReason = record.lastRedirectingRoutingReason;
                            item.HuntPilotDN = record.huntPilotDN;
                            item.HuntPilotPartition = record.huntPilotPartition;
                            item.CalledPartyPatternUsage = record.calledPartyPatternUsage;
                            item.OutpulsedOriginalCalledPartyNum = record.outpulsedOriginalCalledPartyNumber;
                            item.OutpulsedLastRedirectingNum = record.outpulsedLastRedirectingNumber;
                            item.WasCallQueued = record.wasCallQueued;
                            item.TotalWaitTimeInQueue = record.totalWaitTimeInQueue;
                            item.CallingPartyNumUri = record.callingPartyNumber_uri;
                            item.OriginalCalledPartyNumUri = record.originalCalledPartyNumber_uri;
                            item.FinalCalledPartyNumUri = record.finalCalledPartyNumber_uri;
                            item.LastRedirectDnUri = record.lastRedirectDn_uri;
                            item.MobileCallingPartyNum = record.mobileCallingPartyNumber;
                            item.FinalMobileCalledPartyNum = record.finalMobileCalledPartyNumber;
                            item.OrigMobileDeviceName = record.origMobileDeviceName;
                            item.DestMobileDeviceName = record.destMobileDeviceName;
                            item.OrigMobileCallDuration = record.origMobileCallDuration;
                            item.DestMobileCallDuration = record.destMobileCallDuration;
                            item.MobileCallType = record.mobileCallType;
                            item.OrigCalledPartyPattern = record.originalCalledPartyPattern;
                            item.FinalCalledPartyPattern = record.finalCalledPartyPattern;
                            item.LastRedirectingPartyPattern = record.lastRedirectingPartyPattern;
                            item.HuntPilotPattern = record.huntPilotPattern;

                            _iSysBaseServ.iSiscoRecordServ.Add(item);
                        }

                    }

                    await _iSysBaseServ.iSiscoRecordServ.SaveChangesAsync();
                }
            }

            //var fileextension = Path.GetExtension(request.Source.FileName);
            //var filename = Guid.NewGuid().ToString() + fileextension;
            //var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/Sisco", filename);
            //using (FileStream fs = System.IO.File.Create(filepath))
            //{
            //    request.Source.CopyTo(fs);
            //}

            //var records = new List<SiscoBaseModel>();
            //if (fileextension == ".csv")
            //{
            //    using (var reader = new StreamReader(filepath))
            //    {
            //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //        {
            //            records = csv.GetRecords<SiscoBaseModel>().ToList();
            //        }
            //    }
            //}

            //ViewBag.Records = records;

            return true;
        }

        [HttpGet]
        public IActionResult GetNormalNumber(string num)
        {
            var resNum = _iSiscoServ.GetNormalPartyNumber(num);
            return Ok(new
            {
                number = resNum
            });
        }


        [HttpGet]
        public IActionResult GetBaseReport(BaseReportSearch request)
        {
            var query = _iSysBaseServ.iSiscoRecordServ.QueryMaker(y => y.Where(x => true));

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                query = query.Where(x => x.CPartyNumber.Contains(request.PhoneNumber));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = query.Count();
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Menu Category Manage";

            ViewBag.Contents = query.Skip(pager.StartIndex).Take(pager.PageSize).ToList();

            return View(request);
        }
    }
}
