// Utility to extract error messages from API errors
export function getErrorMessage(error: unknown, defaultMessage: string):string {
  if (error && typeof error === 'object' && 'response' in error) {
    const response = (error as { response?: { data?: { message?: string } } }).response;
    if (response?.data?.message) {
      return response.data.message;
    }
  }
  return defaultMessage;
}
