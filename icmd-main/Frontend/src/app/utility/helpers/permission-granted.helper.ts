import { OperationEnum } from "@e/common";

export const isPermissionGranted: (
    currentUserPermissions: ReadonlyArray<string>,
    permission: ReadonlyArray<string>
) => boolean = (
    currentUserPermissions: ReadonlyArray<string>,
    permissions: ReadonlyArray<string>
) => {
        let isGranted: boolean = false;
        permissions.forEach(
            (x) =>
            (isGranted =
                currentUserPermissions
                    .map((s) => (s = s.split("_")[1]))
                    .indexOf(OperationEnum[x]) >= 0)
        );
        return isGranted;
    };
