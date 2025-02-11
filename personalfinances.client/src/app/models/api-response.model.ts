export class APIResponse<T> {
  success: boolean;
  data: T;
  message: string;

  constructor(success: boolean, data: T, message?: string) {
    this.success = success;
    this.data = data;
    this.message = message ? message : '';
  }

  static successResponse<T>(data: T, message?: string): APIResponse<T> {
    return new APIResponse<T>(true, data, message);
  }

  static failResponse<T>(message: string): APIResponse<T> {
    return new APIResponse<T>(false, null as any, message);
  }

  static failResponseWithData<T>(data: T, message: string): APIResponse<T> {
    return new APIResponse<T>(false, data, message);
  }
}
