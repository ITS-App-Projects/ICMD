import { CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";
import { MenuInfoModel } from "@c/menu-role/menu-list-table";

export const mapHelperModelToGetCreateEditMenuModel: (
    model: CreateOrEditMenuModel
) => MenuInfoModel = (model) => ({
    id: model?.id ?? "00000000-0000-0000-0000-000000000000",
    menuName: model?.menuName ?? "",
    menuDescription: model?.menuDescription ?? "",
    controllerName: model?.controllerName ?? "",
    icon: model?.icon ?? "",
    url: model?.url ?? "",
    sortOrder: model?.sortOrder ?? 0,
    parentMenuId: model?.parentMenuId ?? null,
    isActive: true,
    subMenus: []
});
