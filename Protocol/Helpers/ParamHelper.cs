namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;

    internal static class ParamHelper
    {
        public static char[] RestrictedParamNameChars { get; } = { };

        // The restrictedParamNames contains the list of internal params which don't start with two underscores
        private static readonly string[] RestrictedParamNames =
        {
            "TIMEOUT",
            "STATE",
            "New client registered",
            "New Element connection",
            "Client disconnected",
            "Element disconnection",
            "Parameter descriptions",
            "Link file",
            "Edited",
            "Element created",
            "Deleted",
            "Protocol Edited",
            "Alarm Template Assigned",
            "Database optimization",
            "Database stack",
            "Mobile gateway",
            "Service path changed",
            "Startup DataMiner Agent",
            "Protocol Added",
            "Protocol Deleted",
            "Protocol Replaced",
            "Alarm Template Added",
            "Alarm Template Deleted",
            "Alarm Template Edited",
            "Script Added",
            "Script Deleted",
            "Script Edited",
            "Information Added",
            "Information Deleted",
            "Information Edited",
            "SMS Received",
            "SMS Sent",
            "GSM Signal Strength",
            "GSM General Information",
            "Trending Template Edited",
            "Trending Template Added",
            "Trending Template Deleted",
            "VDX Deleted",
            "VDX Added",
            "VDX Edited",
            "Trending Template Assigned",
            "Element Connections Edited",
            "Security Edited",
            "Views Edited",
            "Database settings edited",
            "SNMP-Managers edited",
            "Start Element Failed",
            "Load Element Failed",
            "Table Repair",
            "Set Parameter",
            "Import elements",
            "Information.xml assigned",
            "Start synchronization",
            "Synchronization finished",
            "DataMiner Agent found",
            "DataMiner Agent lost",
            "Error during synchronization",
            "No connection with DMA",
            "Connection established with DMA",
            "Automation info",
            "Scheduler info",
            "Script execution failure",
            "Load Protocol Failed",
            "Startup error",
            "Scheduled Task Created",
            "Scheduled Task Updated",
            "Scheduled Task Deleted",
            "Notification",
            "Stop DataMiner",
            "DataMiner run-time",
            "Task started",
            "Client notification",
            "Set as production protocol",
            "Element masked",
            "Element unmasked",
            "DMS Revisioned",
            "Backup status",
            "SNMPAgent",
            "File changed",
            "Filter added",
            "Filter edited",
            "Filter deleted",
            "User settings",
            "Document added",
            "Document edited",
            "Document removed",
            "Script started",
            "Linked to",
            "State change",
            "Service added",
            "Redundancy Group added",
            "Preset Created",
            "Preset Edited",
            "Preset Renamed",
            "Preset Deleted",
            "Real-time TCP Socket",
            "Database",
            "Correlation engine",
            "Alarm colors edited",
            "IP Settings",
            "Spectrum Script Edited",
            "Spectrum Script Deleted",
            "Spectrum Monitor Edited",
            "Spectrum Monitor Deleted",
            "Entered Prioritized Mode",
            "Left Prioritized Mode",
            "Spectrum Script Added",
            "Spectrum Monitor Created",
            "Mobile Gateway lost contact with DataMiner",
            "Spectrum Monitor Failure",
            "Collaboration Message",
            "DataMiner Failover Status",
            "DMA Redundancy Status",
            "Service Templates",
            "Client Eventing",
            "Latch reset info",
            "Annotations Edited",
            "Asset Manager Configuration",
            "Map Configuration",
            "SNMP Manager",
            "Redundancy Group State",
            "VDX Assigned",
            "Export Progress",
            "Import Progress",
            "Connectivity Engine",
            "Resource Manager info",
            "Tickets",
            "Notes",
            "Profile Manager info",
            "Spectrum trace recording started",
            "Spectrum trace recording stopped",
            "Alarm Group",
            "KQI Engine",
            "DataMiner Connectivity Framework",
            "Authentication Failure",
            "Sounds",
            "Disk Watcher",
            "Licensing",
            "Protocol Function Manager info",
            "lock_status",
            "lock_owner",
            "TotalNbrOfActiveAlarms",
            "TotalNbrOfActiveCriticalAlarms",
            "TotalNbrOfActiveMajorAlarms",
            "TotalNbrOfActiveMinorAlarms",
            "TotalNbrOfActiveWarningAlarms",
            "Element alarm state",
            "TotalNbrOfActiveMaskedAlarms",
            "Nbr of alarms",
            "Execution Verification",
            "ReplicationInfo",
            "ReplicatedElement",
            "RemoteDMAIP",
            "RemoteElementName",
            "ConnectedReplicationDmasCount",
            "ConnectedReplicationDmas",
            "ConnectedReplicationDmasId",
            "ConnectedReplicationDmasReplicationDmaIp",
            "ConnectedReplicationDmasReplicationState",
            "ConnectedReplicationDmasReplicationLastChange",
            "ConnectedReplicationDmasReplicationRemoteElementName",
            "ReplicationDmaRowCountTrigger",
            "ClearTableAfterStartupTrigger",
            "lock_owner_internal",
            "Resource Info",
            "generic_DVE_table",
            "DVE_index",
            "DVE_name",
            "DVE_element",
            "DVE_state",
            "dve_function_guid",
            "DVE_link_resource_manager",
            "generic_interface_table",
            "interface_index",
            "interface_name",
            "interface_type",
            "interface_FK",
            "interface_link_resource_manager",
            "interface_id",
            "generic DVE Linker Table",
            "Linker Index",
            "Linker Generic DVE FK",
            "FK Data",
            "FK Table",
            "DVE_to_delete",
            "dve_delete_add",
            "interface_to_delete",
            "interface_delete_add",
            "New Generic DVE Link",
            "generic dve link to add delete",
        };

        private static readonly Dictionary<uint, (string Name, string Description)> SpectrumParams = new Dictionary<uint, (string Name, string Description)>
        {
            { 64001, ("Trace Data", "Trace Data") },
            { 64003, ("Center Frequency", "Center Frequency") },
            { 64103, ("Center Frequency", "Center Frequency") },
            { 64004, ("Frequency Span", "Frequency Span") },
            { 64104, ("Frequency Span", "Frequency Span") },
            { 64005, ("Start Frequency", "Start Frequency") },
            { 64105, ("Start Frequency", "Start Frequency") },
            { 64006, ("Stop Frequency", "Stop Frequency") },
            { 64106, ("Stop Frequency", "Stop Frequency") },
            { 64009, ("Input Attenuation", "Input Attenuation") },
            { 64109, ("Input Attenuation", "Input Attenuation") },
            { 64010, ("Reference Level", "Reference Level") },
            { 64110, ("Reference Level", "Reference Level") },
            { 64011, ("Reference Scale", "Reference Scale") },
            { 64111, ("Reference Scale", "Reference Scale") },
            { 64012, ("Video Bandwidth", "Video Bandwidth") },
            { 64112, ("Video Bandwidth", "Video Bandwidth") },
            { 64014, ("Resolution Bandwidth", "Resolution Bandwidth") },
            { 64114, ("Resolution Bandwidth", "Resolution Bandwidth") },
            { 64015, ("Sweep Time", "Sweep Time") },
            { 64116, ("Settings Done", "Settings Done") },
            { 64117, ("Number of Users", "Number of Users") },
            { 64118, ("Flag for Set Command", "Flag for Set Command") },
            { 64019, ("Video Bandwidth When Set to Auto", "Video Bandwidth When Set to Auto") },
            { 64020, ("Resolution Bandwidth When Set to Auto", "Resolution Bandwidth When Set to Auto") },
            { 64021, ("Sweep Time When Set to Auto", "Sweep Time When Set to Auto") },
            { 64022, ("Input Attenuation When Set to Auto", "Input Attenuation When Set to Auto") },
            { 64024, ("dBmV Conversion Factor", "dBmV Conversion Factor") },
            { 64025, ("dBuV Conversion Factor", "dBuV Conversion Factor") },
            { 64026, ("Max Wait Time", "Max Wait Time") },
            { 64027, ("Inhibit Measure Flag", "Inhibit Measure Flag") },
            { 64028, ("Spectrum Display String", "Spectrum Display String") },
            { 64031, ("Detection Mode", "Detection Mode") },
            { 64131, ("Detection Mode", "Detection Mode") },
            { 64032, ("Scale Type", "Scale Type") },
            { 64132, ("Scale Type", "Scale Type") },
            { 64036, ("Over Response NBW", "Over Response NBW") },
            { 64037, ("Amount of Points", "Amount of Points") },
        };

        private static readonly Dictionary<uint, (string Name, string Description)> SlaParams2_0_0 = new Dictionary<uint, (string Name, string Description)>
        {
            /* Currently based on: https://svn.skyline.be/svn/SystemEngineering/Protocols/Skyline/Skyline SLA Definition Basic/2.0.0.33 */
            /* Note that a version 2.0.0.34 exists on SVN but has not been validated by Software */

            { 1   , ("read_service_name", "Service Name") },
            { 2   , ("bus_service_id", "Service id") },
            { 3   , ("not_used", "not_used") },
            { 4   , ("raw_alarm_input", "raw_alarm_input") },
            { 5   , ("value 0", "") },
            { 6   , ("value 1", "") },
            { 7   , ("value from creation", "") },
            { 8   , ("value forever", "") },
            { 10  , ("read_sla_compliance_Status", "Compliance") },
            { 11  , ("read_service_alarm_status", "Service Alarm State") },
            { 13  , ("read_sla_predicted_compliance_Status", "Predicted Compliance") },
            { 14  , ("history property update", "history property update") },
            { 20  , ("read_total_time_left_before_breach", "Total Violation Time Left") },
            { 21  , ("read_total_time_violated", "Total Violation Time") },
            { 22  , ("read_max_violation_time", "Longest Violation Time") },
            { 24  , ("read_number_of_violation", "Number of Violations") },
            { 25  , ("read_number_of_affecting_alarms", "Number of Affecting Alarms") },
            { 27  , ("read_reset_counters", "Last Manual Reset") },
            { 28  , ("write_reset_counters", "") },
            { 29  , ("base_timestamp", "Base Timestamp") },
            { 30  , ("base_timestamp", "Base Timestamp") },
            { 31  , ("read_current_timestamp_base", "Start Time") },
            { 32  , ("read_next_timestamp_base", "End Time") },
            { 34  , ("read_percent_time_violated", "Violation percentage") },
            { 35  , ("sla_state", "Admin State") },
            { 36  , ("sla_state", "Admin State") },
            { 37  , ("array_alarm_store", "Outage List") },
            { 38  , ("column_severity", "Alarm Severity") },
            { 39  , ("column_timestamp", "Begin Timestamp") },
            { 40  , ("column_admin_state", "Adm. State") },
            { 41  , ("read_max_time_left_before_breach", "Single Violation Time Left") },
            { 42  , ("read_breach_time_availability", "Total Violation Time Availability") },
            { 43  , ("read_consecutive_breach_time_availability", "Single Violation Time Availability") },
            { 44  , ("read_number_of_violations_left", "Number of Violations Left") },
            { 45  , ("read_violation_count_availability", "Number of Violations Availability") },
            { 3045, ("read_violation_count_difference", "Number of Unavailable Violations") },
            { 46  , ("column_end_timestamp", "End Timestamp") },
            { 47  , ("column_index", "column_index") },
            { 48  , ("read_non_percent_time_violated", "Availability") },
            { 3048, ("read_non_percent_time_voilated_difference", "Unavailability") },
            { 49  , ("read_predicted_non_percent_time_violated", "Predicted Availability") },
            { 50  , ("column_motivation", "Motivation") },
            { 51  , ("column_motivation", "Motivation") },
            { 52  , ("column_correction", "Correction") },
            { 53  , ("column_correction", "Correction") },
            { 54  , ("column_outage", "Outage") },
            { 55  , ("convert", "convert") },
            { 56  , ("column_outage_pct", "Outage pct") },
            { 57  , ("column_correction__pct", "Correction pct") },
            { 58  , ("last_outage_index", "last_outage_index") },
            { 59  , ("monitorspan in sec", "monitor span") },
            { 60  , ("column_outage_corrected", "Outage Corrected") },
            { 61  , ("column_violation_pct", "Violation pct") },
            { 62  , ("column_window_status", "Current window") },
            { 63  , ("months_to_keep_outages", "Time to Keep Outages") },
            { 64  , ("months_to_keep_outages", "Time to Keep Outages") },
            { 65  , ("read_non_percent_time_violated_without_correction", "Availability Without Correction") },
            { 3065, ("read_non_percent_time_violated_without_correction_difference", "Unavailability Without Correction") },
            { 66  , ("TicketNr", "Ticket") },
            { 67  , ("sla_validity_start_time", "SLA Validity Start Time") },
            { 68  , ("sla_validity_end_time", "SLA Validity End Time") },
            { 69  , ("sla_validity_start_time", "SLA Validity Start Time") },
            { 71  , ("sla_validity_end_time", "SLA Validity End Time") },
            { 70  , ("sla_health_status", "SLA health status") },
            { 72  , ("service_live_service_state", "Service Live State") },
            { 73  , ("service_current_violation_impact", "Current Outage Impact") },
            { 74  , ("column_impact", "Outage impact") },
            { 75  , ("read_unweight_total_time_violated", "Total Outage Time") },
            { 76  , ("read_unweight_max_violation_time", "Longest Outage Time") },
            { 77  , ("column_violation", "Violation") },
            { 78  , ("array_alarm_store_ContextMenu", "Outage List_ContextMenu") },
            { 79  , ("array_alarm_store_QActionFeedback", "Outage List_QActionFeedback") },
            { 101 , ("sla_monitor_time", "Time") },
            { 102 , ("sla_monitor_time", "Time") },
            { 103 , ("sla_monitor_unit", "Unit") },
            { 104 , ("sla_monitor_unit", "Unit") },
            { 105 , ("sla_monitor_type", "Type") },
            { 106 , ("sla_monitor_type", "Type") },
            { 108 , ("sla_delay_time", "Delay Time") },
            { 109 , ("sla_delay_time", "Delay Time") },
            { 110 , ("sla_recalculate", "recalculate") },
            { 111 , ("sla_minimum_outage_threshold", "Minimum Outage Threshold") },
            { 112 , ("sla_minimum_outage_threshold", "Minimum Outage Threshold") },
            { 121 , ("sla_breach_value", "Maximum Total Violations Value") },
            { 122 , ("sla_breach_value", "Maximum Total Violations Value") },
            { 123 , ("sla_breach_unit", "Maximum Total Violations Unit") },
            { 124 , ("sla_breach_unit", "Maximum Total Violations Unit") },
            { 125 , ("sla_consecutive_breach_value", "Maximum Single Violation Value") },
            { 126 , ("sla_consecutive_breach_value", "Maximum Single Violation Value") },
            { 127 , ("sla_consecutive_breach_unit", "Maximum Single Violation Unit") },
            { 128 , ("sla_consecutive_breach_unit", "Maximum Single Violation Unit") },
            { 129 , ("sla_max_violations", "Total Violations Before Breach") },
            { 130 , ("sla_max_violations", "Total Violations Before Breach") },
            { 131 , ("sla_violation_level", "Violation Level") },
            { 132 , ("sla_violation_level", "Violation Level") },
            { 133 , ("manual_outage_start_time", "Manual outage start time") },
            { 134 , ("manual_outage_start_time", "Manual outage start time") },
            { 135 , ("manual_outage_end_time", "Manual outage end time") },
            { 136 , ("manual_outage_end_time", "Manual outage end time") },
            { 137 , ("manual_outage_motivation", "Manual outage motivation") },
            { 138 , ("manual_outage_motivation", "Manual outage motivation") },
            { 139 , ("manual_outage_overrule_motivation", "Overrule existing motivations") },
            { 140 , ("manual_outage_overrule_motivation", "Overrule existing motivations") },
            { 141 , ("manual_outage_add", "") },
            { 142 , ("manual_outage_delete", "") },
            { 143 , ("manual_outage_delete_pk", "Manual outage key") },
            { 144 , ("manual_outage_delete_pk", "Manual outage key") },
            { 147 , ("manual_outage_start_time_client_input", "New Outage Start Time") },
            { 148 , ("manual_outage_start_time_client_input", "New Outage Start Time") },
            { 149 , ("manual_outage_end_time_client_input", "New Outage End Time") },
            { 150 , ("manual_outage_end_time_client_input", "New Outage End Time") },
            { 166 , ("TicketNr", "Ticket") },
            { 201 , ("sla_total_breach", "Maximum Total Violation Time") },
            { 203 , ("sla_max_consecutive_breach", "Maximum Single Violation Time") },
            { 208 , ("sla_total_slot_type", "Maximum Total Violations Type") },
            { 209 , ("sla_total_slot_type", "Maximum Total Violations Type") },
            { 210 , ("sla_max_consecutive_slot_type", "Maximum Single Violation Type") },
            { 211 , ("sla_max_consecutive_slot_type", "Maximum Single Violation Type") },
            { 212 , ("total_relative_percentage", "Maximum Total Violations Percentage") },
            { 213 , ("total_relative_percentage", "Maximum Total Violations Percentage") },
            { 216 , ("max_consecutive_relative_percentage", "Maximum Single Violation Percentage") },
            { 217 , ("max_consecutive_relative_percentage", "Maximum Single Violation Percentage") },
            { 250 , ("Current_array_active_service_alarms", "Current Active Service Alarms") },
            { 251 , ("Current_array_active_service_alarms_id", "Current Active Service Alarm Id") },
            { 252 , ("Current_array_active_service_alarms_severity", "Current Active Service Alarm Severity") },
            { 253 , ("Current_array_active_service_alarms_time", "Current Active Service Alarm Time") },
            { 254 , ("Current_array_active_service_alarms_element", "Current Active Service Alarm Element") },
            { 255 , ("Current_array_active_service_alarms_parameter", "Current Active Service Alarm Parameter") },
            { 256 , ("Current_array_active_service_alarms_value", "Current Active Service Alarm Value") },
            { 257 , ("Current_array_active_service_alarms_state", "Current Active Service Alarm State") },
            { 258 , ("Current_array_active_service_alarms_type", "Current Active Service Alarm Type") },
            { 259 , ("Current_array_active_service_alarms_user_state", "Current Active Service Alarm User State") },
            { 260 , ("Current_array_active_service_alarms_source", "Current Active Service Alarm Source") },
            { 261 , ("Current_array_active_service_alarms_category", "Current Active Service Alarm Category") },
            { 262 , ("Current_array_active_service_alarms_offline impact", "Current Active Service Alarm Offline Impact") },
            { 263 , ("Current_array_active_service_alarms_service_point", "Current Active Service Alarm Service Point") },
            { 264 , ("Current_array_active_service_alarms_component_info", "Current Active Service Alarm Component Info") },
            { 265 , ("Current_array_active_service_alarms_inclusion_state", "Current Active Service Alarm Overruled Inclusion State") },
            { 266 , ("Current_array_active_service_alarms_inclusion_state", "Current Active Service Alarm Overruled Inclusion State") },
            { 267 , ("Current_array_active_service_alarms_calculated_inclusion_state", "Current Active Service Alarm Calculated Inclusion State") },
            { 300 , ("title_begin_sla_status", "Compliance Info") },
            { 301 , ("title_end_sla_status", "") },
            { 302 , ("title_begin_service_status", "General Info") },
            { 303 , ("title_end_service_status", "") },
            { 304 , ("title_begin_violation_status", "Performance Indicators") },
            { 305 , ("title_end_violation_status", "") },
            { 306 , ("title_begin_sla_window", "Window settings") },
            { 307 , ("title_end_sla_window", "") },
            { 308 , ("title_begin_sla_config", "Extra settings") },
            { 309 , ("title_end_sla_config", "") },
            { 310 , ("title_begin_total_breach_config", "Total violation") },
            { 311 , ("title_end_total_breach_config", "") },
            { 312 , ("title_begin_cons_breach_config", "Single violation") },
            { 313 , ("title_end_cons_breach_config", "") },
            { 314 , ("title_begin_number_breach_config", "Violation count") },
            { 315 , ("title_end_number_breach_config", "") },
            { 316 , ("title_begin_sla_alarm_config", "Alarm settings") },
            { 317 , ("title_end_sla_alarm_config", "") },
            { 318 , ("title_begin_advanced_config", "Advanced Config") },
            { 319 , ("title_end_advanced_config", "") },
            { 350 , ("trigger_dummy_row_added", "trigger_dummy_row_added") },
            { 351 , ("trigger_dummy_row_changed", "trigger_dummy_row_changed") },
            { 352 , ("trigger_dummy_row_deleted", "trigger_dummy_row_deleted") },
            { 400 , ("array_offline_window", "Offline Window") },
            { 401 , ("array_offline_window_id", "Offline Window Id") },
            { 402 , ("array_offline_window_start_day", "Offline Window Start Day") },
            { 403 , ("array_offline_window_start_time", "Offline Window Start Time") },
            { 405 , ("array_offline_window_end_time", "Offline Window End Time") },
            { 406 , ("array_offline_window_state", "Offline Window State") },
            { 412 , ("array_offline_window_start_day", "Offline Window Start Day") },
            { 413 , ("array_offline_window_start_time", "Offline Window Start Time") },
            { 414 , ("array_offline_window_end_day", "Offline Window End Day") },
            { 415 , ("array_offline_window_end_time", "Offline Window End Time") },
            { 416 , ("array_offline_window_state", "Offline Window State") },
            { 420 , ("array_offline_window_change", "Offline Window Change") },
            { 450 , ("array_violation_settings", "Violation Settings") },
            { 451 , ("array_violation_settings_id", "Violation Filter Id") },
            { 452 , ("array_violation_settings_type", "Violation Filter Type") },
            { 453 , ("array_violation_settings_value", "Violation Filter Value") },
            { 454 , ("array_violation_settings_impact", "Violation Filter Impact") },
            { 455 , ("array_violation_sequence", "Violation Filter Sequence") },
            { 456 , ("array_violation_state", "Violation Filter State") },
            { 457 , ("array_violation_exclusive", "Violation Filter Exclusive") },
            { 458 , ("array_violation_property_name", "Violation Filter Property Name") },
            { 462 , ("array_violation_settings_type", "Violation Filter Type") },
            { 463 , ("array_violation_settings_value", "Violation Filter Value") },
            { 464 , ("array_violation_settings_impact", "Violation Filter Impact") },
            { 465 , ("array_violation_sequence", "Violation Filter Sequence") },
            { 466 , ("array_violation_state", "Violation Filter State") },
            { 467 , ("array_violation_exclusive", "Violation Filter Exclusive") },
            { 468 , ("array_violation_property_name", "Violation Filter Property Name") },
            { 470 , ("array_violation_setting_add_entry", "Violation Settings") },
            { 500 , ("OutageDetails", "OutageDetails") },
            { 550 , ("array_root_to_outage", "table holding root to outage") },
            { 551 , ("array_root_to_outage_id", "array_root_to_outage_id") },
            { 552 , ("array_root_to_outage_root", "array_root_to_outage_root") },
            { 553 , ("array_root_to_outage_outage", "array_root_to_outage_outage") },
            { 554 , ("array_root_to_outage_weight", "array_root_to_outage_weight") },
            { 555 , ("array_root_to_outage_inclusion_state", "array_root_to_outage_inclustion_state") },
            { 556 , ("array_root_to_outage_alarm", "array_root_to_outage_alarm") },
            { 600 , ("GenerateTicket", "Generate Ticket") },
            { 610 , ("GenerateTicket", "Generate Ticket") },
            { 750 , ("array_active_service_alarms", "Active Service Alarms") },
            { 751 , ("array_active_service_alarms_rootid", "Active Service Alarm RootId") },
            { 752 , ("array_active_service_alarms_severity", "Active Service Alarm Severity") },
            { 753 , ("array_active_service_alarms_time", "Active Service Alarm Time") },
            { 754 , ("array_active_service_alarms_element", "Active Service Alarm Element") },
            { 755 , ("array_active_service_alarms_parameter", "Active Service Alarm Parameter") },
            { 756 , ("array_active_service_alarms_value", "Active Service Alarm Value") },
            { 757 , ("array_active_service_alarms_state", "Active Service Alarm State") },
            { 758 , ("array_active_service_alarms_type", "Active Service Alarm Type") },
            { 759 , ("array_active_service_alarms_user_state", "Active Service Alarm User State") },
            { 760 , ("array_active_service_alarms_source", "Active Service Alarm Source") },
            { 761 , ("array_active_service_alarms_category", "Active Service Alarm Category") },
            { 762 , ("array_active_service_alarms_offline impact", "Active Service Alarm Offline Impact") },
            { 763 , ("array_active_service_alarms_service_point", "Active Service Alarm Service Point") },
            { 764 , ("array_active_service_alarms_component_info", "Active Service Alarm Component Info") },
            { 765 , ("array_active_service_alarms_inclusion_state", "Active Service Alarm Overruled Inclusion State") },
            { 766 , ("array_active_service_alarms_inclusion_state", "Active Service Alarm Overruled Inclusion State") },
            { 767 , ("array_active_service_alarms_calculated_inclusion_state", "Active Service Alarm Calculated Inclusion State") },
            { 768 , ("array_active_service_alarms_id", "Active Service Alarm Id") },
            { 1000, ("History Statistics Table", "History Statistics Table") },
            { 1001, ("Tracking Period", "Tracking Period [IDX]") },
            { 1002, ("Start Time (History)", "Start Time (History)") },
            { 1003, ("End Time (History)", "End Time (History)") },
            { 1004, ("Stored End Time (History)", "Stored End Time (History)") },
            { 1005, ("Compliance (History)", "Compliance (History)") },
            { 1006, ("Availability (History)", "Availability (History)") },
            { 1007, ("Availability Without Corrections (History)", "Availability Without Corrections (History)") },
            { 1008, ("Total Violation Time (History)", "Total Violation Time (History)") },
            { 1009, ("Longest Violation Time (History)", "Longest Violation Time (History)") },
            { 1010, ("Number Of Violations (History)", "Number of Violations (History)") },
            { 1050, ("Clear History Statistics Table", "") },
            { 1051, ("Current History IDX", "Current History IDX" ) },
            { 1052, ("Max Number Of History Rows", "Max Number of History Rows") },
            { 1053, ("Max Number Of History Rows", "Max Number of History Rows") },
            { 1060, ("ConfigurationName", "Configuration Name") },
            { 1061, ("ConfigurationName", "Configuration Name") },
            { 1062, ("AvailableConfigurations", "AvailableConfigurations") },
            { 1063, ("SaveLoadConfig", "") },
            { 1064, ("Save/Load Status", "Save/Load Status") },
            { 1065, ("PB_SaveLoadConfig", "") },
            { 1066, ("BUT_LoadConfigurations", "") },
            { 2000, ("pagebutton Edit Outage", "") },
            { 2001, ("Outage_to_Stop_Delete", "Outage to Stop or Delete") },
            { 2002, ("Outage_to_Stop_Delete", "Outage to Stop or Delete") },
            { 2003, ("Delete Selected Outage", "") },
            { 2050, ("hide_filtered_alarms", "Hide filtered alarms") },
            { 2051, ("hide_filtered_alarms", "Hide filtered alarms") },
        };

        private static readonly Dictionary<uint, (string Name, string Description)> SlaParams3_0_0 = new Dictionary<uint, (string Name, string Description)>
        {
            /* Currently based on: https://svn.skyline.be/svn/SystemEngineering/Protocols/Skyline/Skyline SLA Definition Basic/3.0.0.10 */
            /* Note that more recent versions up to 3.0.0.15 exist on SVN but have not been validated by Software */

            { 1   , ("read_service_name", "Service Name") },
            { 2   , ("bus_service_id", "Service id") },
            { 3   , ("not_used", "not_used") },
            { 4   , ("raw_alarm_input", "raw_alarm_input") },
            { 5   , ("value 0", "") },
            { 6   , ("value 1", "") },
            { 7   , ("value from creation", "") },
            { 8   , ("value forever", "") },
            { 10  , ("read_sla_compliance_Status", "Compliance") },
            { 11  , ("read_service_alarm_status", "Service Alarm State") },
            { 13  , ("read_sla_predicted_compliance_Status", "Predicted Compliance") },
            { 14  , ("history property update", "history property update") },
            { 20  , ("read_total_time_left_before_breach", "Total Violation Time Left") },
            { 21  , ("read_total_time_violated", "Total Violation Time") },
            { 22  , ("read_max_violation_time", "Longest Violation Time") },
            { 24  , ("read_number_of_violation", "Number of Violations") },
            { 25  , ("read_number_of_affecting_alarms", "Number of Affecting Alarms") },
            { 27  , ("read_reset_counters", "Last Manual Reset") },
            { 28  , ("write_reset_counters", "") },
            { 29  , ("base_timestamp", "Base Timestamp") },
            { 30  , ("base_timestamp", "Base Timestamp") },
            { 31  , ("read_current_timestamp_base", "Start Time") },
            { 32  , ("read_next_timestamp_base", "End Time") },
            { 34  , ("read_percent_time_violated", "Violation percentage") },
            { 35  , ("sla_state", "Admin State") },
            { 36  , ("sla_state", "Admin State") },
            { 37  , ("array_alarm_store", "Outage List") },
            { 38  , ("column_severity", "Alarm Severity") },
            { 39  , ("column_timestamp", "Begin Timestamp") },
            { 40  , ("column_admin_state", "Adm. State") },
            { 41  , ("read_max_time_left_before_breach", "Single Violation Time Left") },
            { 42  , ("read_breach_time_availability", "Total Violation Time Availability") },
            { 43  , ("read_consecutive_breach_time_availability", "Single Violation Time Availability") },
            { 44  , ("read_number_of_violations_left", "Number of Violations Left") },
            { 45  , ("read_violation_count_availability", "Number of Violations Availability") },
            { 3045, ("read_violation_count_difference", "Number of Unavailable Violations") },
            { 46  , ("column_end_timestamp", "End Timestamp") },
            { 47  , ("column_index", "column_index") },
            { 48  , ("read_non_percent_time_violated", "Availability") },
            { 3048, ("read_non_percent_time_voilated_difference", "Unavailability") },
            { 49  , ("read_predicted_non_percent_time_violated", "Predicted Availability") },
            { 50  , ("column_motivation", "Motivation") },
            { 51  , ("column_motivation", "Motivation") },
            { 52  , ("column_correction", "Correction") },
            { 53  , ("column_correction", "Correction") },
            { 54  , ("column_outage", "Outage") },
            { 55  , ("convert", "convert") },
            { 56  , ("column_outage_pct", "Outage pct") },
            { 57  , ("column_correction__pct", "Correction pct") },
            { 58  , ("last_outage_index", "last_outage_index") },
            { 59  , ("monitorspan in sec", "monitor span") },
            { 60  , ("column_outage_corrected", "Outage Corrected") },
            { 61  , ("column_violation_pct", "Violation pct") },
            { 62  , ("column_window_status", "Current window") },
            { 63  , ("months_to_keep_outages", "Time to Keep Outages") },
            { 64  , ("months_to_keep_outages", "Time to Keep Outages") },
            { 65  , ("read_non_percent_time_violated_without_correction", "Availability Without Correction") },
            { 3065, ("read_non_percent_time_violated_without_correction_difference", "Unavailability Without Correction") },
            { 66  , ("TicketNr", "Ticket") },
            { 67  , ("sla_validity_start_time", "SLA Validity Start Time") },
            { 68  , ("sla_validity_end_time", "SLA Validity End Time") },
            { 69  , ("sla_validity_start_time", "SLA Validity Start Time") },
            { 71  , ("sla_validity_end_time", "SLA Validity End Time") },
            { 70  , ("sla_health_status", "SLA health status") },
            { 72  , ("service_live_service_state", "Service Live State") },
            { 73  , ("service_current_violation_impact", "Current Outage Impact") },
            { 74  , ("column_impact", "Outage impact") },
            { 75  , ("read_unweight_total_time_violated", "Total Outage Time") },
            { 76  , ("read_unweight_max_violation_time", "Longest Outage Time") },
            { 77  , ("column_violation", "Violation") },
            { 78  , ("array_alarm_store_ContextMenu", "Outage List_ContextMenu") },
            { 79  , ("array_alarm_store_QActionFeedback", "Outage List_QActionFeedback") },
            { 101 , ("sla_monitor_time", "Time") },
            { 102 , ("sla_monitor_time", "Time") },
            { 103 , ("sla_monitor_unit", "Unit") },
            { 104 , ("sla_monitor_unit", "Unit") },
            { 105 , ("sla_monitor_type", "Type") },
            { 106 , ("sla_monitor_type", "Type") },
            { 108 , ("sla_delay_time", "Delay Time") },
            { 109 , ("sla_delay_time", "Delay Time") },
            { 110 , ("sla_recalculate", "recalculate") },
            { 111 , ("sla_minimum_outage_threshold", "Minimum Outage Threshold") },
            { 112 , ("sla_minimum_outage_threshold", "Minimum Outage Threshold") },
            { 121 , ("sla_breach_value", "Maximum Total Violations Value") },
            { 122 , ("sla_breach_value", "Maximum Total Violations Value") },
            { 123 , ("sla_breach_unit", "Maximum Total Violations Unit") },
            { 124 , ("sla_breach_unit", "Maximum Total Violations Unit") },
            { 125 , ("sla_consecutive_breach_value", "Maximum Single Violation Value") },
            { 126 , ("sla_consecutive_breach_value", "Maximum Single Violation Value") },
            { 127 , ("sla_consecutive_breach_unit", "Maximum Single Violation Unit") },
            { 128 , ("sla_consecutive_breach_unit", "Maximum Single Violation Unit") },
            { 129 , ("sla_max_violations", "Total Violations Before Breach") },
            { 130 , ("sla_max_violations", "Total Violations Before Breach") },
            { 131 , ("sla_violation_level", "Violation Level") },
            { 132 , ("sla_violation_level", "Violation Level") },
            { 133 , ("manual_outage_start_time", "Manual outage start time") },
            { 134 , ("manual_outage_start_time", "Manual outage start time") },
            { 135 , ("manual_outage_end_time", "Manual outage end time") },
            { 136 , ("manual_outage_end_time", "Manual outage end time") },
            { 137 , ("manual_outage_motivation", "Manual outage motivation") },
            { 138 , ("manual_outage_motivation", "Manual outage motivation") },
            { 139 , ("manual_outage_overrule_motivation", "Overrule existing motivations") },
            { 140 , ("manual_outage_overrule_motivation", "Overrule existing motivations") },
            { 141 , ("manual_outage_add", "") },
            { 142 , ("manual_outage_delete", "") },
            { 143 , ("manual_outage_delete_pk", "Manual outage key") },
            { 144 , ("manual_outage_delete_pk", "Manual outage key") },
            { 147 , ("manual_outage_start_time_client_input", "New Outage Start Time") },
            { 148 , ("manual_outage_start_time_client_input", "New Outage Start Time") },
            { 149 , ("manual_outage_end_time_client_input", "New Outage End Time") },
            { 150 , ("manual_outage_end_time_client_input", "New Outage End Time") },
            { 166 , ("TicketNr", "Ticket") },
            { 201 , ("sla_total_breach", "Maximum Total Violation Time") },
            { 203 , ("sla_max_consecutive_breach", "Maximum Single Violation Time") },
            { 208 , ("sla_total_slot_type", "Maximum Total Violations Type") },
            { 209 , ("sla_total_slot_type", "Maximum Total Violations Type") },
            { 210 , ("sla_max_consecutive_slot_type", "Maximum Single Violation Type") },
            { 211 , ("sla_max_consecutive_slot_type", "Maximum Single Violation Type") },
            { 212 , ("total_relative_percentage", "Maximum Total Violations Percentage") },
            { 213 , ("total_relative_percentage", "Maximum Total Violations Percentage") },
            { 216 , ("max_consecutive_relative_percentage", "Maximum Single Violation Percentage") },
            { 217 , ("max_consecutive_relative_percentage", "Maximum Single Violation Percentage") },
            { 250 , ("Current_array_active_service_alarms", "Current Active Service Alarms") },
            { 251 , ("Current_array_active_service_alarms_id", "Current Active Service Alarm Id") },
            { 252 , ("Current_array_active_service_alarms_severity", "Current Active Service Alarm Severity") },
            { 253 , ("Current_array_active_service_alarms_time", "Current Active Service Alarm Time") },
            { 254 , ("Current_array_active_service_alarms_element", "Current Active Service Alarm Element") },
            { 255 , ("Current_array_active_service_alarms_parameter", "Current Active Service Alarm Parameter") },
            { 256 , ("Current_array_active_service_alarms_value", "Current Active Service Alarm Value") },
            { 257 , ("Current_array_active_service_alarms_state", "Current Active Service Alarm State") },
            { 258 , ("Current_array_active_service_alarms_type", "Current Active Service Alarm Type") },
            { 259 , ("Current_array_active_service_alarms_user_state", "Current Active Service Alarm User State") },
            { 260 , ("Current_array_active_service_alarms_source", "Current Active Service Alarm Source") },
            { 261 , ("Current_array_active_service_alarms_category", "Current Active Service Alarm Category") },
            { 262 , ("Current_array_active_service_alarms_offline impact", "Current Active Service Alarm Offline Impact") },
            { 263 , ("Current_array_active_service_alarms_service_point", "Current Active Service Alarm Service Point") },
            { 264 , ("Current_array_active_service_alarms_component_info", "Current Active Service Alarm Component Info") },
            { 265 , ("Current_array_active_service_alarms_inclusion_state", "Current Active Service Alarm Overruled Inclusion State") },
            { 266 , ("Current_array_active_service_alarms_inclusion_state", "Current Active Service Alarm Overruled Inclusion State") },
            { 267 , ("Current_array_active_service_alarms_calculated_inclusion_state", "Current Active Service Alarm Calculated Inclusion State") },
            { 300 , ("title_begin_sla_status", "Compliance Info") },
            { 301 , ("title_end_sla_status", "") },
            { 302 , ("title_begin_service_status", "General Info") },
            { 303 , ("title_end_service_status", "") },
            { 304 , ("title_begin_violation_status", "Performance Indicators") },
            { 305 , ("title_end_violation_status", "") },
            { 306 , ("title_begin_sla_window", "Window settings") },
            { 307 , ("title_end_sla_window", "") },
            { 308 , ("title_begin_sla_config", "Extra settings") },
            { 309 , ("title_end_sla_config", "") },
            { 310 , ("title_begin_total_breach_config", "Total violation") },
            { 311 , ("title_end_total_breach_config", "") },
            { 312 , ("title_begin_cons_breach_config", "Single violation") },
            { 313 , ("title_end_cons_breach_config", "") },
            { 314 , ("title_begin_number_breach_config", "Violation count") },
            { 315 , ("title_end_number_breach_config", "") },
            { 316 , ("title_begin_sla_alarm_config", "Alarm settings") },
            { 317 , ("title_end_sla_alarm_config", "") },
            { 318 , ("title_begin_advanced_config", "Advanced Config") },
            { 319 , ("title_end_advanced_config", "") },
            { 350 , ("trigger_dummy_row_added", "trigger_dummy_row_added") },
            { 351 , ("trigger_dummy_row_changed", "trigger_dummy_row_changed") },
            { 352 , ("trigger_dummy_row_deleted", "trigger_dummy_row_deleted") },
            { 400 , ("array_offline_window", "Offline Window") },
            { 401 , ("array_offline_window_id", "Offline Window Id") },
            { 402 , ("array_offline_window_start_day", "Offline Window Start Day") },
            { 403 , ("array_offline_window_start_time", "Offline Window Start Time") },
            { 405 , ("array_offline_window_end_time", "Offline Window End Time") },
            { 406 , ("array_offline_window_state", "Offline Window State") },
            { 412 , ("array_offline_window_start_day", "Offline Window Start Day") },
            { 413 , ("array_offline_window_start_time", "Offline Window Start Time") },
            { 414 , ("array_offline_window_end_day", "Offline Window End Day") },
            { 415 , ("array_offline_window_end_time", "Offline Window End Time") },
            { 416 , ("array_offline_window_state", "Offline Window State") },
            { 420 , ("array_offline_window_change", "Offline Window Change") },
            { 450 , ("array_violation_settings", "Violation Settings") },
            { 451 , ("array_violation_settings_id", "Violation Filter Id") },
            { 452 , ("array_violation_settings_type", "Violation Filter Type") },
            { 453 , ("array_violation_settings_value", "Violation Filter Value") },
            { 454 , ("array_violation_settings_impact", "Violation Filter Impact") },
            { 455 , ("array_violation_sequence", "Violation Filter Sequence") },
            { 456 , ("array_violation_state", "Violation Filter State") },
            { 457 , ("array_violation_exclusive", "Violation Filter Exclusive") },
            { 458 , ("array_violation_property_name", "Violation Filter Property Name") },
            { 462 , ("array_violation_settings_type", "Violation Filter Type") },
            { 463 , ("array_violation_settings_value", "Violation Filter Value") },
            { 464 , ("array_violation_settings_impact", "Violation Filter Impact") },
            { 465 , ("array_violation_sequence", "Violation Filter Sequence") },
            { 466 , ("array_violation_state", "Violation Filter State") },
            { 467 , ("array_violation_exclusive", "Violation Filter Exclusive") },
            { 468 , ("array_violation_property_name", "Violation Filter Property Name") },
            { 470 , ("array_violation_setting_add_entry", "Violation Settings") },
            { 500 , ("OutageDetails", "OutageDetails") },
            { 550 , ("array_root_to_outage", "table holding root to outage") },
            { 551 , ("array_root_to_outage_id", "array_root_to_outage_id") },
            { 552 , ("array_root_to_outage_root", "array_root_to_outage_root") },
            { 553 , ("array_root_to_outage_outage", "array_root_to_outage_outage") },
            { 554 , ("array_root_to_outage_weight", "array_root_to_outage_weight") },
            { 555 , ("array_root_to_outage_inclusion_state", "array_root_to_outage_inclustion_state") },
            { 556 , ("array_root_to_outage_alarm", "array_root_to_outage_alarm") },
            { 600 , ("GenerateTicket", "Generate Ticket") },
            { 610 , ("GenerateTicket", "Generate Ticket") },
            { 750 , ("array_active_service_alarms", "Active Service Alarms") },
            { 751 , ("array_active_service_alarms_rootid", "Active Service Alarm RootId") },
            { 752 , ("array_active_service_alarms_severity", "Active Service Alarm Severity") },
            { 753 , ("array_active_service_alarms_time", "Active Service Alarm Time") },
            { 754 , ("array_active_service_alarms_element", "Active Service Alarm Element") },
            { 755 , ("array_active_service_alarms_parameter", "Active Service Alarm Parameter") },
            { 756 , ("array_active_service_alarms_value", "Active Service Alarm Value") },
            { 757 , ("array_active_service_alarms_state", "Active Service Alarm State") },
            { 758 , ("array_active_service_alarms_type", "Active Service Alarm Type") },
            { 759 , ("array_active_service_alarms_user_state", "Active Service Alarm User State") },
            { 760 , ("array_active_service_alarms_source", "Active Service Alarm Source") },
            { 761 , ("array_active_service_alarms_category", "Active Service Alarm Category") },
            { 762 , ("array_active_service_alarms_offline impact", "Active Service Alarm Offline Impact") },
            { 763 , ("array_active_service_alarms_service_point", "Active Service Alarm Service Point") },
            { 764 , ("array_active_service_alarms_component_info", "Active Service Alarm Component Info") },
            { 765 , ("array_active_service_alarms_inclusion_state", "Active Service Alarm Overruled Inclusion State") },
            { 766 , ("array_active_service_alarms_inclusion_state", "Active Service Alarm Overruled Inclusion State") },
            { 767 , ("array_active_service_alarms_calculated_inclusion_state", "Active Service Alarm Calculated Inclusion State") },
            { 768 , ("array_active_service_alarms_id", "Active Service Alarm Id") },
            { 1000, ("History Statistics Table", "History Statistics Table") },
            { 1001, ("Tracking Period", "Tracking Period [IDX]") },
            { 1002, ("Start Time (History)", "Start Time (History)") },
            { 1003, ("End Time (History)", "End Time (History)") },
            { 1004, ("Stored End Time (History)", "Stored End Time (History)") },
            { 1005, ("Compliance (History)", "Compliance (History)") },
            { 1006, ("Availability (History)", "Availability (History)") },
            { 1007, ("Availability Without Corrections (History)", "Availability Without Corrections (History)") },
            { 1008, ("Total Violation Time (History)", "Total Violation Time (History)") },
            { 1009, ("Longest Violation Time (History)", "Longest Violation Time (History)") },
            { 1010, ("Number Of Violations (History)", "Number of Violations (History)") },
            { 1050, ("Clear History Statistics Table", "") },
            { 1051, ("Current History IDX", "Current History IDX" ) },
            { 1052, ("Max Number Of History Rows", "Max Number of History Rows") },
            { 1053, ("Max Number Of History Rows", "Max Number of History Rows") },
            { 1060, ("ConfigurationName", "Configuration Name") },
            { 1061, ("ConfigurationName", "Configuration Name") },
            { 1062, ("AvailableConfigurations", "AvailableConfigurations") },
            { 1063, ("SaveLoadConfig", "") },
            { 1064, ("Save/Load Status", "Save/Load Status") },
            { 1065, ("PB_SaveLoadConfig", "") },
            { 1066, ("BUT_LoadConfigurations", "") },
            { 2000, ("pagebutton Edit Outage", "") },
            { 2001, ("Outage_to_Stop_Delete", "Outage to Stop or Delete") },
            { 2002, ("Outage_to_Stop_Delete", "Outage to Stop or Delete") },
            { 2003, ("Delete Selected Outage", "") },
            { 2050, ("hide_violation_filtered_alarms", "Violation Filtered Alarms") },
            { 2051, ("hide_violation_filtered_alarms", "Violation Filtered Alarms") },
            { 2052, ("hide_offline_window_outages", "Offline Window Outages") },
            { 2053, ("hide_offline_window_outages", "Offline Window Outages") },
            { 2054, ("enable_predictions", "Predictions") },
            { 2055, ("enable_predictions", "Predictions") },
            { 2056, ("enable_outages", "Outages") },
            { 2057, ("enable_outages", "Outages") },
            { 2058, ("show_active_alarms", "Active Alarms") },
            { 2059, ("show_active_alarms", "Active Alarms") },
            { 2060, ("enable_enhanced_service_mode", "Enhanced Service Mode" ) },
            { 2062, ("show_excluded_service_element_alarms", "Excluded Service Element Alarms Visibility") },
            { 2063, ("show_excluded_service_element_alarms", "Excluded Service Element Alarms Visibility") },
            { 2064, ("use_service_capping", "Service Capping") },
            { 2065, ("use_service_capping", "Service Capping") },
            { 2066, ("Affecting_Alarms_Level", "Affecting Alarms Level") },
            { 2067, ("Affecting_Alarms_Level", "Affecting Alarms Level") },
        };

        private static readonly Dictionary<uint, (string Name, string Description)> EnhancedServiceParams = new Dictionary<uint, (string Name, string Description)>
        {
            /* Currently based on: https://svn.skyline.be/svn/SystemEngineering/Protocols/Skyline/Skyline Service Definition Basic/1.0.0.7 */
            
            {1, ("Service Name", "Service Name") },
            {2, ("Service Severity", "Service Severity") },
            {3, ("Severity Update", "Severity Update") },
            {4, ("raw_alarm_input", "raw_alarm_input") },
            {5, ("subservice_element_update", "Severity Update (Element in Sub Service)") },

            // Table
            {100, ("Service Element Status", "Service Element Status") },
            {101, ("Service_Element_Status_Alias", "Alias [IDX] (Service Element Status)") },
            {102, ("Service_Element_Status_Element_Name", "Element Name (Service Element Status)") },
            {103, ("Service_Element_Status_Element_ID", "Element ID (Service Element Status)") },
            {104, ("Service_Element_Status_Severity", "Severity (Service Element Status)") },
            {105, ("Service_Element_Status_State", "State (Service Element Status)") },
            {106, ("Service_Element_Status_Calculated", "Calculated (Service Element Status)") },
            {107, ("Service_Element_Status_Parent_Service_Name", "Parent Service Name (Service Element Status)") },
            {108, ("Service_Element_Status_Parent_Service_ID", "Parent Service ID (Service Element Status)") },
            {109, ("Service_Element_Status_Service_Index", "Service Index (Service Element Status)") },
            {110, ("Service_Element_Status_Alarm_Count_Critical", "Critical (Service Element Status)") },
            {111, ("Service_Element_Status_Alarm_Count_Major", "Major (Service Element Status)") },
            {112, ("Service_Element_Status_Alarm_Count_Minor", "Minor (Service Element Status)") },
            {113, ("Service_Element_Status_Alarm_Count_Warning", "Warning (Service Element Status)") },
            {114, ("Service_Element_Status_Alarm_Count_Timeout", "Timeout (Service Element Status)") },
            {115, ("Service_Element_Type", "Type (Service Element Status)") },

            {198, ("monitor_active_alarms", "Monitor Active Alarms") },
            {199, ("monitor_active_alarms", "Monitor Active Alarms") },

            // Table
            {200, ("active_service_alarms", "Active Service Alarms") },
            {201, ("active_service_alarms_root_alarm_ID", "Root Alarm ID [IDX] (Active Service Alarms)") },
            {202, ("active_service_alarms_alarm_ID", "Alarm ID (Active Service Alarms)") },
            {203, ("active_service_alarms_severity", "Severity (Active Service Alarms)") },
            {204, ("active_service_alarms_time", "Time (Active Service Alarms)") },
            {205, ("active_service_alarms_element", "Element Name (Active Service Alarms)") },
            {206, ("active_service_alarms_parameter_description", "Parameter Description (Active Service Alarms)") },
            {207, ("active_service_alarms_param_index", "Parameter Key (Active Service Alarms)") },
            {208, ("active_service_alarms_value", "Value (Active Service Alarms)") },
            {209, ("active_service_alarms_status", "Status (Active Service Alarms)") },
            {210, ("active_service_alarms_alarm_type", "Alarm Type (Active Service Alarms)") },
            {211, ("active_service_alarms_user_state", "User Status (Active Service Alarms)") },
            {212, ("active_service_alarms_source", "Source (Active Service Alarms)") },
            {213, ("active_service_alarms_category", "Category (Active Service Alarms)") },
        };

        private static readonly Dictionary<uint, (string Name, string Description)> GeneralParams = new Dictionary<uint, (string Name, string Description)>
        {
            { 65000, ("[Lock Status]", "[Lock Status]") },
            { 65002, ("[Lock owner]", "[Lock owner]") },
            { 65003, ("[Number of active Alarms]", "[Number of active Alarms]") },
            { 65004, ("[Number of active Critical Alarms]", "[Number of active Critical Alarms]") },
            { 65005, ("[Number of active Major Alarms]", "[Number of active Major Alarms]") },
            { 65006, ("[Number of active Minor Alarms]", "[Number of active Minor Alarms]") },
            { 65007, ("[Number of active Warning Alarms]", "[Number of active Warning Alarms]") },
            { 65008, ("[Element Alarm state]", "[Element Alarm state]") },
            { 65009, ("[Number of masked Alarms]", "[Number of masked Alarms]") },
            { 65017, ("[Timer base]", "[Timer base]") },
            { 65019, ("[Properties]", "[Properties]") },
            { 65020, ("[Property name]", "[Property name]") },
            { 65021, ("[Property type]", "[Property type]") },
            { 65022, ("[Property Value]", "[Property Value]") },
            { 65026, ("[Element id]", "[Element id]") },
            { 65027, ("[RCA Level]", "[RCA Level]") },
            { 65029, ("[Clients connected]", "[Clients connected]") },
            { 65030, ("[Priority level]", "[Priority level]") },
            { 65031, ("[Priority level]", "[Priority level]") },
            { 65032, ("[Latch state]", "[Latch state]") },
            { 65033, ("[Reset element latch]", "[Reset element latch]") },
            { 65034, ("[Communication info]", "[Communication info]") },
            { 65035, ("[Connection ID]", "[Connection ID]") },
            { 65036, ("[Device RTT]", "[Device RTT]") },
            { 65037, ("[Device Iterations]", "[Device Iterations]") },
            { 65038, ("[DataMiner TX]", "[DataMiner TX]") },
            { 65039, ("[DataMiner RX]", "[DataMiner RX]") },
            { 65040, ("[Session DataMiner TX]", "[Session DataMiner TX]") },
            { 65041, ("[Session DataMiner RX]", "[Session DataMiner RX]") },
            { 65042, ("[Device Message Drops]", "[Device Message Drops]") },
            { 65043, ("[Communication info state]", "[Communication info state]") },
            { 65044, ("[Communication info state]", "[Communication info state]") },
            { 65045, ("[Connection State]", "[Connection State]") },
            { 65046, ("Execution Verification", "Execution Verification") },
            { 65047, ("[Connection Name]", "[Connection Name]") },
            { 65048, ("[Connection Type]", "[Connection Type]") },
            { 65049, ("[Interfaces]", "[Interfaces]") },
            { 65050, ("[Interface ID]", "[Interface ID]") },
            { 65051, ("[Interface Name]", "[Interface Name]") },
            { 65052, ("[Interface Type]", "[Interface Type]") },
            { 65053, ("[Interface Alarm State]", "[Interface Alarm State]") },
            { 65054, ("[Interface Properties]", "[Interface Properties]") },
            { 65055, ("[Interface Property name]", "[Interface Property name]") },
            { 65056, ("[Interface Property type]", "[Interface Property type]") },
            { 65057, ("[Interface Property value]", "[Interface Property value]") },
            { 65058, ("[Interface Property value]", "[Interface Property value]") },
            { 65059, ("[Interface Property link]", "[Interface Property link]") },
            { 65060, ("[Connections]", "[Connections]") },
            { 65061, ("[Connections ID]", "[Connections ID]") },
            { 65062, ("[Source Interface]", "[Source Interface]") },
            { 65064, ("[Destination Interface]", "[Destination Interface]") },
            { 65065, ("[Source Interface]", "[Source Interface]") },
            { 65067, ("[Destination Interface]", "[Destination Interface]") },
            { 65068, ("[Connection Properties]", "[Connection Properties]") },
            { 65069, ("[Connection Property Name]", "[Connection Property Name]") },
            { 65070, ("[Connection Property Type]", "[Connection Property Type]") },
            { 65071, ("[Connection Property value]", "[Connection Property value]") },
            { 65072, ("[Connection Property value]", "[Connection Property value]") },
            { 65073, ("[Connection Property link]", "[Connection Property link]") },
            { 65074, ("[Add Connection]", "[Add Connection]") },
            { 65075, ("[Add Interface Property]", "[Add Interface Property]") },
            { 65076, ("[Add Connection Property]", "[Add Connection Property]") },
            { 65077, ("[Interface Property to delete]", "[Interface Property to delete]") },
            { 65078, ("[Connection to delete]", "[Connection to delete]") },
            { 65079, ("[Connection to delete]", "[Connection to delete]") },
            { 65080, ("[Connection Property to delete]", "[Connection Property to delete]") },
            { 65081, ("[Connection Property to delete]", "[Connection Property to delete]") },
            { 65082, ("[Interface Property ID]", "[Interface Property ID]") },
            { 65083, ("[Connection Property ID]", "[Connection Property ID]") },
            { 65084, ("[Interface Property to delete]", "[Interface Property to delete]") },
            { 65085, ("[Interface Property name]", "[Interface Property name]") },
            { 65086, ("[Interface Property type]", "[Interface Property type]") },
            { 65087, ("[Connection Property Name]", "[Connection Property Name]") },
            { 65088, ("[Connection Property type]", "[Connection Property type]") },
            { 65089, ("[Destination DataMiner/Element]", "[Destination DataMiner/Element]") },
            { 65090, ("[Destination DataMiner/Element]", "[Destination DataMiner/Element]") },
            { 65091, ("[Interface Property link]", "[Interface Property link]") },
            { 65092, ("[Connection Property link]", "[Connection Property link]") },
            { 65093, ("[Custom Name]", "[Custom Name]") },
            { 65094, ("[Custom Name]", "[Custom Name]") },
            { 65095, ("[Interface Dynamic Link]", "[Interface Dynamic Link]") },
            { 65096, ("[Connections Name]", "[Connections Name]") },
            { 65098, ("[Interface Properties Input]", "[Interface Properties Input]") },
            { 65099, ("[Connections Input]", "[Connections Input]") },
            { 65100, ("[Connections Properties Input]", "[Connections Properties Input]") },
            { 65101, ("[Connections Filter]", "[Connections Filter]") },
            { 65102, ("[Connections Filter]", "[Connections Filter]") },
            { 65103, ("DataMiner Connectivity Framework", "DataMiner Connectivity Framework") },
            { 65105, ("[DataMiner Availability]", "[DataMiner Availability]") },
            { 65106, ("[DataMiner Availability ID]", "[DataMiner Availability ID]") },
            { 65107, ("[DataMiner Availability From]", "[DataMiner Availability From]") },
            { 65108, ("[DataMiner Availability To]", "[DataMiner Availability To]") },
            { 65109, ("[DataMiner Availability Reason]", "[DataMiner Availability Reason]") },
            { 65110, ("[DataMiner Availability Reference]", "[DataMiner Availability Reference]") },
            { 65111, ("[DataMiner Availability From]", "[DataMiner Availability From]") },
            { 65112, ("[DataMiner Availability To]", "[DataMiner Availability To]") },
            { 65113, ("[DataMiner Availability Reason]", "[DataMiner Availability Reason]") },
            { 65114, ("[DataMiner Availability Reference]", "[DataMiner Availability Reference]") },
            { 65118, ("[Replicated Element]", "[Replicated Element]") },
            { 65119, ("[Remote DMA IP]", "[Remote DMA IP]") },
            { 65120, ("[Remote Element Name]", "[Remote Element Name]") },
            { 65121, ("[Connected Replication DMAs Count]", "[Connected Replication DMAs Count]") },
            { 65122, ("[Connected Replication DMAs]", "[Connected Replication DMAs]") },
            { 65123, ("[ID]", "[ID]") },
            { 65124, ("[DMA IP]", "[DMA IP]") },
            { 65125, ("[Replication State]", "[Replication State]") },
            { 65126, ("[Last Change]", "[Last Change]") },
            { 65127, ("[Replicated Element Name]", "[Replicated Element Name]") },
            { 65130, ("[Internal Lock owner]", "[Internal Lock owner]") }
        };

        public static bool IsCorrectSlaParam(IParamsParam checkParam)
        {
            if (checkParam == null)
            {
                throw new ArgumentNullException(nameof(checkParam));
            }

            if (checkParam.Id?.Value == null)
            {
                throw new InvalidDataException("Parameter ID is invalid.");
            }

            uint paramId = checkParam.Id.Value.Value;
            string paramName = checkParam.Name?.Value;
            string paramDescription = checkParam.Description?.Value;
            if (SlaParams2_0_0.TryGetValue(paramId, out var param2) && paramName == param2.Name && paramDescription == param2.Description)
            {
                return true;
            }

            if (SlaParams3_0_0.TryGetValue(paramId, out var param3) && paramName == param3.Name && paramDescription == param3.Description)
            {
                return true;
            }

            return false;
        }

        public static bool IsCorrectEnhancedServiceParam(IParamsParam checkParam)
        {
            if (checkParam == null)
            {
                throw new ArgumentNullException(nameof(checkParam));
            }

            if (checkParam.Id?.Value == null)
            {
                throw new InvalidDataException("Parameter ID is invalid.");
            }

            uint paramId = checkParam.Id.Value.Value;
            string paramName = checkParam.Name?.Value;
            string paramDescription = checkParam.Description?.Value;
            if (EnhancedServiceParams.TryGetValue(paramId, out var param) && paramName == param.Name && paramDescription == param.Description)
            {
                return true;
            }

            return false;
        }

        public static bool IsRestrictedParamName(IParamsParam checkParam)
        {
            if (checkParam == null)
            {
                throw new ArgumentNullException(nameof(checkParam));
            }

            string paramName = checkParam.Name?.Value;

            // Check if Name starts with two underscore (such name are reserved for software internal use only)
            return (paramName != null && paramName.StartsWith("__")) || RestrictedParamNames.Contains(paramName, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsCorrectSpectrumParam(IParamsParam checkParam)
        {
            if (checkParam == null)
            {
                throw new ArgumentNullException(nameof(checkParam));
            }

            if (checkParam.Id?.Value == null)
            {
                throw new InvalidDataException("Parameter ID is invalid.");
            }

            uint paramId = checkParam.Id.Value.Value;
            string paramName = checkParam.Name?.Value;
            string paramDescription = checkParam.Description?.Value;
            if (SpectrumParams.TryGetValue(paramId, out var param) && paramName == param.Name && paramDescription == param.Description)
            {
                return true;
            }

            return false;
        }

        public static bool IsGeneralParam(IParamsParam checkParam)
        {
            if (checkParam == null)
            {
                throw new ArgumentNullException(nameof(checkParam));
            }

            if (checkParam.Id?.Value == null)
            {
                throw new InvalidDataException("Parameter ID is invalid.");
            }

            uint paramId = checkParam.Id.Value.Value;
            return GeneralParams.ContainsKey(paramId);
        }

        public static bool IsGeneralParam(uint paramId)
        {
            return GeneralParams.ContainsKey(paramId);
        }

        public static bool IsInternalPid(int paramId)
        {
            if ((paramId >= 64300 && paramId <= 69999) || (paramId >= 100000 && paramId <= 999999))
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<object> GetParamNameUnrecommendedChars(string paramName)
        {
            HashSet<object> usedUnrecommendedChars = new HashSet<object>();

            for (int i = 0; i < paramName.Length; i++)
            {
                if (Char.IsLetterOrDigit(paramName[i]) || paramName[i] == '_')
                {
                    continue;
                }

                if (Char.IsWhiteSpace(paramName[i]))
                {
                    usedUnrecommendedChars.Add("[Whitespace]");
                }
                else
                {
                    usedUnrecommendedChars.Add(paramName[i]);
                }
            }

            return usedUnrecommendedChars;
        }

        public static IEnumerable<char> GetParamNameUnrecommendedStartChars(string paramName)
        {
            List<char> usedUnrecommendedStartChars = new List<char>();

            for (int i = 0; i < paramName.Length; i++)
            {
                if (Char.IsDigit(paramName[i]))
                {
                    usedUnrecommendedStartChars.Add(paramName[i]);
                }
                else if (Char.IsLetter(paramName[i]))
                {
                    break;
                }
            }

            return usedUnrecommendedStartChars;
        }

        public static string ReplaceParamNameInvalidChars(string paramName)
        {
            string newName = paramName;

            for (int i = 0; i < paramName.Length; i++)
            {
                if (!Char.IsLetterOrDigit(paramName[i]) && paramName[i] != ' ' && paramName[i] != '_')
                {
                    newName = paramName.Replace(paramName[i], '_');
                }
            }

            return newName;
        }

        public static string ReplaceParamNameUnrecommendedChars(string paramName)
        {
            List<char> newNameChars = new List<char>();
            bool isNextCharToUpper = false;

            if (paramName.StartsWith(" "))
            {
                newNameChars.Add('_');
                isNextCharToUpper = true;
            }

            for (int i = 0; i < paramName.Length; i++)
            {
                if (Char.IsWhiteSpace(paramName[i]))
                {
                    isNextCharToUpper = true;
                    continue;
                }

                if (!Char.IsLetterOrDigit(paramName[i]) && paramName[i] != '_')
                {
                    newNameChars.Add('_');
                    isNextCharToUpper = true;
                    continue;
                }

                if (isNextCharToUpper)
                {
                    newNameChars.Add(Char.ToUpper(paramName[i]));
                }
                else
                {
                    newNameChars.Add(paramName[i]);
                }

                isNextCharToUpper = false;
            }

            return String.Join("", newNameChars);
        }

        public static bool TryFindTableParamForColumnPid(IProtocolModel model, string columnPid, out IParamsParam tableParam)
        {
            if (model.TryGetObjectByKey(Mappings.ParamsById, columnPid, out IParamsParam columnParam)
                && columnParam.TryGetTable(model.RelationManager, out tableParam))
            {
                return true;
            }

            tableParam = null;
            return false;
        }

        public static bool IsValidParamAssociation(IList<IParamsParam> parameters)
        {
            if (parameters.Count == 1)
            {
                return true;
            }

            if (parameters.Count > 2)
            {
                return false;
            }

            if (IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.Read, EnumParamType.Write) ||

                IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.Read, EnumParamType.WriteBit) ||
                IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.ReadBit, EnumParamType.Write) ||
                IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.ReadBit, EnumParamType.WriteBit) ||

                IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.Header, EnumParamType.Write) ||
                IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.Trailer, EnumParamType.Write) ||

                IsParamsComboOfTypes(parameters[0], parameters[1], EnumParamType.Array, EnumParamType.Write) /* Matrix */)
            {
                return true;
            }

            return false;
        }

        private static bool IsParamsComboOfTypes(IParamsParam param1, IParamsParam param2, EnumParamType validComboType1, EnumParamType validComboType2)
        {
            return param1.Type?.Value == validComboType1 && param2.Type?.Value == validComboType2 ||
                   param1.Type?.Value == validComboType2 && param2.Type?.Value == validComboType1;
        }

        /// <summary>
        /// Check if the provided column is of type SNMP.
        /// In case there is no table specified, it will be retrieved.
        /// </summary>
        public static bool IsColumnOfTypeSnmp(IParamsParam column, RelationManager relationManager, IParamsParam table = null)
        {
            if (column == null)
            {
                return false;
            }

            if (table == null && !column.TryGetTable(relationManager, out table))
            {
                // Couldn't find the correct table.
                return false;
            }

            if (table.ArrayOptions == null || table.ArrayOptions.Count == 0)
            {
                // Table has no columns defined.
                return false;
            }

            var columnOption = table.ArrayOptions.FirstOrDefault(typeColumnOption => typeColumnOption.Pid?.Value == column.Id?.Value);

            return columnOption?.Type?.Value == EnumColumnOptionType.Snmp;
        }
    }
}