namespace Talo.Templating;

public static class BuiltinTemplates
{
    public const string AdrInitTemplate =
        """
        # {{SEQUENCE-NUMBER}}. {{TITLE}}

        ## Status

        | Status                   | Time               |
        |--------------------------|--------------------|
        | {{STATUS}}               | {{TIME}}           |
        
        ## Context

        We need to record the architectural decisions made on this project.

        ## Decision

        We will use Architecture Decision Records, as described by Michael Nygard in this article: http://thinkrelevance.com/blog/2011/11/15/documenting-architecture-decisions

        ## Consequences

        See Michael Nygard's article, linked above.
        """;

    public const string AdrTemplate =
        """
        # {{SEQUENCE-NUMBER}}. {{TITLE}}

        ## Status

        | Status                   | Time               |
        |--------------------------|--------------------|
        | {{STATUS}}               | {{TIME}}           |

        ## Context


        ## Decision


        ## Consequences


        """;

    public const string RfcTemplate =
        """
        # {{SEQUENCE-NUMBER}}. {{TITLE}}
        
        ## Status
        
        | Status                   | Time               |
        |--------------------------|--------------------|
        | {{STATUS}}               | {{TIME}}           |
        
        ## Summary

        ## Background

        ## Problem

        ## Proposal

        ## Alternatives Considered and Trade-offs

        ## Definition of success


        """;
}
