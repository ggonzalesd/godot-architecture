namespace StructProject.Core.Shared.Service;

public interface ILogger
{
  /// <summary>
  /// Logs a message to the appropriate logging mechanism.
  /// </summary>
  /// <param name="message">The message to log.</param>
  void Log(params object[] message);

  /// <summary>
  /// Logs an error message to the appropriate logging mechanism.
  /// </summary>
  /// <param name="message">The error message to log.</param>
  /// <remarks>
  /// This method is intended for logging error messages. It may include additional context or formatting specific to error logging, depending on the implementation.
  /// </remarks>
  void Error(params object[] message)
  {
    Log(message);
  }
}
