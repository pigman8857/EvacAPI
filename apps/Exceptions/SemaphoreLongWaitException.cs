
namespace evacPlanMoni.apps.Exceptions;

public class SemaphoreLongWaitException : Exception
{
  public SemaphoreLongWaitException() : base("Please try again later")
  {

  }

}