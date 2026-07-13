export interface PagedResponse<T> {

    items: T[];

    pageNumber: number;

    pageSize: number;

    totalRecords: number;

    totalPages: number;

    hasNextPage: boolean;

    hasPreviousPage: boolean;

}