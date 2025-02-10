using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.Constants
{
    public enum TagFieldSource
    {
        [Display(Name = "Not Used")]
        NotUsed,
        [Display(Name = "Tag Field Table 1")]
        Process,
        [Display(Name = "Tag Field Table 2")]
        SubProcess,
        [Display(Name = "Tag Field Table 3")]
        Stream,
        [Display(Name = "Manually Hand Typed")]
        HandTyped,
        [Display(Name = "Tag Type Id")]
        TagTypeId,
        [Display(Name = "Descriptor")]
        Descriptor,
        [Display(Name = "Equipment Code")]
        EquipmentCode
    }

    public enum RecordType
    {
        Active,
        Inactive,
        All
    }

    public enum AttributeType
    {
        Text,
        Integer,
        Decimal
    }

    public enum AuthorizationTypes
    {
        [Display(Name = "Read Only")]  // first one is default
        ReadOnly,
        [Display(Name = "Read Write")]
        ReadWrite,
        [Display(Name = "Administrator")]
        Administrator
    }

    public enum SearchType
    {
        [Display(Name = "Contains")]
        Contains,
        [Display(Name = "StartsWith")]
        StartsWith,
        [Display(Name = "EndsWith")]
        EndsWith,
        [Display(Name = "Equals")]
        Equals,
        [Display(Name = "DoesNotContains")]
        DoesNotContains,
        [Display(Name = "DoesNotEquals")]
        DoesNotEquals
    }

    public enum HierachyType
    {
        [Display(Name = "Control Hierarchy")]
        Control,
        [Display(Name = "CCMD Hierarchy")]
        CCMD,
        [Display(Name = "Cable Hierarchy")]
        Cable
    }

    public enum Options
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Inactive")]
        Inactive,
        [Display(Name = "All")]
        All
    }

    public class IsInstrumentOption
    {
        public const String Yes = "Y";
        public const String No = "N";
        public const String Both = "B";
        public const String None = "-";
    }

    public enum ChangeLogOptions
    {
        ActiveDeactive,
        Deleted
    }

    public enum FileType
    {
        Invalid = 0,
        OMItems = 1,
        EquipmentList = 2,
        InstrumentList = 3,
        ValveList = 4,
        CCMD = 5,
        OMServiceDescriptions = 6,

        Bank = 7,
        WorkAreaPack = 8,
        Train = 9,
        Zone = 10,
        System = 11,
        SubSystem = 12,
        TagField1 = 13,
        TagField2 = 14,
        TagField3 = 15,
        ReferenceDocument = 16,
        Tags = 17,
        JunctionBox = 18,
        Panel = 19,
        Skid = 20,
        Stand = 21,
        ReferenceDocumentType = 22,
        EquipmentCode = 23,
        FailState = 24,
        TagType = 25,
        TagDescriptor = 26,
        Manufacturer = 27,
        DeviceModel = 28,
        DeviceType = 29,
        NatureOfSignals = 30
    }

    public enum PnIdDeviceMisMatchDocumentReference
    {
        PnID_Device_MismatchedDocumentNumber,
        PnID_Device_MismatchedDocumentNumber_VersionRevision,
        PnID_Device_MismatchedDocumentNumber_VersionRevisionInclNulls
    }

    public enum Operations
    {
        Add = 1,
        Edit = 2,
        Delete = 3,
        ActiveInActive = 4,
        Download = 5,
    }

    public class ImportFileRecordStatus
    {
        public const String Success = "Success";
        public const String Fail = "Fail";
    }

    public class OperationType
    {
        public const String Insert = "INSERT";
        public const String Edit = "EDIT";
    }

    public class ResponseMessages
    {
        #region Account

        public const string LoginInvalid = "Login credentials are invalid";
        public const string AccountDeactivate = "Account is deactivated";
        public const string LoginSuccess = "Sucessfully Login";
        public const string LoginFail = "Failed to login";
        public const string UserRegisterSuccess = "User register successfully.";
        public const string ProjectNotAssigned = "No project has been assigned. Please contact the system administrator.";
        #endregion

        #region User
        public const string UserCreated = "User created successfully.";
        public const string UserNotCreated = "User not created, Please try again !";
        public const string RoleNotCreated = "Role not created, Please try again !";
        public const string UsernameAlreadyTaken = "User name is already taken!";
        public const string UserNotExist = "User does not exist, Please enter valid user id !";
        public const string UserUpdated = "User updated successfully.";
        public const string UserNotUpdated = "User not updated, Please try again !";
        public const string EmailAlreadyTaken = "Email is already taken!";
        public const string PhoneNoAlreadyTaken = "Phone Number is already taken!";
        public const string UserDeleted = "User deleted successfully.";
        public const string UserNotDeleted = "User not deleted, Please try again !";
        public const string UserNotDeleteAlreadyAssigned = "A user cannot be deleted while it is assigned to a project.";
        public const string PasswordChanged = "Password changed successfully.";
        public const string PasswordNotChanged = "Password has not been changed, Please try again later !";
        public const string CurrentPasswordInValid = "Your current password is incorrect. Please enter a valid current password.";
        #endregion

        #region Project
        public const string ProjectCreated = "Project created successfully.";
        public const string ProjectNotCreated = "Project not created, Please try again !";
        public const string ProjectNotExist = "Project does not exist, Please enter valid project id !";
        public const string ProjectUpdated = "Project updated successfully.";
        public const string ProjectNotUpdated = "Project not updated, Please try again !";
        public const string ProjectDeleted = "Project deleted successfully.";
        public const string ProjectNotDeleted = "Project not deleted, Please try again !";
        public const string ProjectActivate = "Project activated successfully.";
        public const string ProjectDeactivate = "Project inactivated successfully.";
        public const string ProjectTagFieldUpdated = "Project tag fields updated successfully.";
        #endregion

        #region Device
        #endregion

        #region Bank
        public const string BankAlreadyTaken = "Bank is already taken!";
        #endregion

        #region WorkAreaPack
        public const string NumberAlreadyTaken = "Number is already taken!";
        public const string WorkAreaPackNotDelete = "A work area pack cannot be deleted while it is assigned to a system.";
        #endregion

        #region Train
        public const string TrainAlreadyTaken = "Train is already taken!";
        #endregion

        #region Zone
        public const string ZoneAlreadyTaken = "Zone is already taken!";
        #endregion

        #region System
        public const string SystemNotDeleteAlreadyAssigned = "A system cannot be deleted while it is assigned to a sub-system.";
        #endregion

        #region DocumentType
        public const string TypeNotDeleteAlreadyAssigned = "A reference document type cannot be deleted while it is assigned to a reference document.";
        public const string ManufacturerNotDelete = "A manufacturer cannot be deleted while it is assigned to a model.";
        #endregion

        #region Device
        public const string DeviceDeactived = "A device cannot be deactivated while it is the Connection Parent or Instrument Parent of another active device.";
        public const string DeviceActivated = "A device cannot be activated while it's Connection Parent or Instrument Parent is inactive.";
        #endregion

        #region Import
        public const string FailedOMImport = "File is not in the right format for uploading.  Please upload a valid .csv file with the correct content for O&M Items import.";
        public const string ImportFile = "File import successfully.";
        public const string FailedPnIdImport = "File is not in the right format for uploading.  Please upload a valid .csv file with the correct content for P&ID import.";
        public const string MissingTagImport = "File was imported successfully, however the following tags couldn't be found in the system and thus their CCMD details were not imported: {{missingTags}} . To fix this, create these tags in the system, assign them to a device and run the import again.";
        public const string FailedImportFile = "File import failed. Kindly check results for details.";
        public const string SomeFailedImportFile = "File was imported successfully, however some records have failed. Kindly check results for details.";
        #endregion

        #region Menu
        public const string NoMenuPermission = "No menu selected for permission.";
        public const string PermissionUpdate = "Permission updated successfully.";
        public const string MenuAlreadyExist = "Menu already exist.";
        #endregion

        #region Common

        //Model Attribute Validation Messages
        public const string ParentDeviceNotSame = "Connection and Instrument parent device can't be the same.";
        public const string EnterValidModule = "Please enter a valid {module}.";
        public const string ModuleActivate = "{module} activated successfully.";
        public const string ModuleDeactivate = "{module} deactivated successfully.";
        public const string ModuleTagNotDeleteAlreadyAssigned = "A {module} cannot be deleted while it is connected to a device.";
        public const string ModuleTagNotDeactivatedAlreadyAssigned = "A {module} cannot be deactived while it is connected to a device.";
        public const string TagNameAlreadyTaken = "Tag name is already taken!";
        public const string TagNotDeleteAlreadyAssigned = "A tag cannot be deleted while it is assigned to an entity in the system such as a device, stand or cable.";
        public const string NatureSignalNameAlreadyTaken = "Nature of signal name is already taken!";
        public const string ModelAlreadyTaken = "Model is already taken!";
        public const string FailStateNameAlreadyTaken = "Fail state name is already taken!";
        public const string ManufacturerNameAlreadyTaken = "Manufacturer name is already taken!";
        public const string CodeAlreadyTaken = "Code is already taken!";
        public const string ModuleNotDeleteAlreadyAssigned = "A {module} cannot be deleted while it is assigned to a device.";
        public const string ModuleNotDeleteAlreadyAssignedTag = "A {module} cannot be deleted while it is assigned to a tag.";
        public const string AlreadyUsedNotDelete = "A {module} can't delete it because already used.";
        public const string ProjectNameAlreadyTaken = "Project name is already taken!";
        public const string DocumentNumberAlreadyTaken = "Document number is already taken!";
        public const string StreamNameAlreadyTaken = "Stream name is already taken!";
        public const string ProcessNameAlreadyTaken = "Process name is already taken!";
        public const string SubProcessNameAlreadyTaken = "Sub process name is already taken!";
        public const string TypeAlreadyTaken = "Type is already taken!";
        public const string ModuleCreated = "{module} created successfully.";
        public const string ModuleNotCreated = "{module} not created, Please try again !";
        public const string ModuleNotExist = "{module} does not exist, Please enter valid id !";
        public const string ModuleUpdated = "{module} updated successfully.";
        public const string ModuleNotUpdated = "{module} not updated, Please try again !";
        public const string ModuleDeleted = "{module} deleted successfully.";
        public const string ModuleNotDeleted = "{module} not deleted, Please try again !";
        public const string ModuleAlreadyTaken = "{{module}} is already taken!";
        public const string ModuleNotValid = "A {module} does not exist, Please enter a valid {module}!";
        public const string DateIsNotValid = "{module} does not valid, Please enter a valid date!";


        public const string UserAuthorizationsRequired = "The field UserAuthorizations is required.";
        public const string URLIsInvalid = "The field url is invalid.";
        public const string StringFieldLength = "The {0} must be at least {2} and at max {1} characters long.";
        public const string GlobalModelValidationMessage = "Request model isn't valid";
        public const string NameAlreadyTaken = "Name is already taken!";
        public const string GenerateTag = "Generate tag successfully.";

        public const string ColumnNotSelected = "Please select columns.";
        public const string PrefixMetaDataTableForColumnTemplate = "Column Template.";
        #endregion

    }

    public class MetaDataColumnTemplate
    {
        public static readonly List<string> DefaultTemplates = new()
        {
            $"{ResponseMessages.PrefixMetaDataTableForColumnTemplate}CCMD",
            $"{ResponseMessages.PrefixMetaDataTableForColumnTemplate}Task Force",
            $"{ResponseMessages.PrefixMetaDataTableForColumnTemplate}PSS",
        };
    }

    public class MemoryCacheItems
    {
        public static readonly List<string> cacheItems = new()
        {
            IdentityClaimNames.MenuAndPermission,
            "InstrumentList",
            "NonInstrumentList",
            "ManageUsers",
            "ManageProjects",
            "WorkAreaPack",
            "Zones",
            "Systems",
            "SubSystem",
            "TagField1",
            "TagField2",
            "TagField3",
            "ReferenceDocument",
            "Tags",
            "JunctionBox",
            "Panel",
            "Skid",
            "Stand",
            "EquipmentCode",
            "TagTypes",
            "TagDescriptors",
            "Manufacturer",
            "DeviceModel",
            "DeviceType"
        };
    }
}
