import { UseFormSetError, FieldValues, Path } from 'react-hook-form';
import { ApiError } from '@/shared/types/api.types';

/**
 * Maps versioned backend validation dictionaries into deeply nested React Hook Form fields.
 * Gracefully translates C# PascalCase/array paths (e.g., "Members[0].Email" or "Organization.Address.Street")
 * into dynamic Javascript paths (e.g., "members.0.email" or "organization.address.street").
 */
export function mapApiErrorsToForm<TFieldValues extends FieldValues>(
  error: ApiError,
  setError: UseFormSetError<TFieldValues>
): void {
  if (!error.errors) return;

  Object.entries(error.errors).forEach(([backendPath, messages]) => {
    // 1. Convert C# array notation "Property[0]" to dot-index notation "property.0"
    // 2. Split by dots to handle nested structures
    // 3. Lowercase first letter of each nested segment to follow javascript camelCase properties
    const parsedPath = backendPath
      .replace(/\[(\d+)\]/g, '.$1') // Converts [0] to .0
      .split('.')
      .map((part) => {
        // If part is a numeric index, preserve it
        if (/^\d+$/.test(part)) return part;
        return part.charAt(0).toLowerCase() + part.slice(1);
      })
      .join('.');

    setError(parsedPath as Path<TFieldValues>, {
      type: 'server',
      message: messages.join(' '),
    });
  });
}
