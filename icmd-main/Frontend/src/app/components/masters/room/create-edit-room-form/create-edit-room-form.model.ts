export interface CreateOrEditRoomDtoModel {
    id: string;
    projectId: string | null;
    room: string | null;
    description: string | null;
}