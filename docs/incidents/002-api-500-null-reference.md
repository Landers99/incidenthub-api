# Incident 002: API 500 Null Reference

## Summary
The incident detail endpoint returned 500 Internal Server Error for incidents that did not have an assigned user.

## Symptoms
- Assigned incidents loaded successfully.
- Unassigned incidents failed.
- Logs showed a NullReferenceException during response mapping.

## Impact
Clients could not view valid incident records when the incident had no assignee.

## Investigation
1. Created an incident with assignedToUserId set to null 
2. Called detail endpoint.
3. Found that the request failed during mapping from Incident to IncidentResponse.

## Root Couse
The response mapper assumed Incident.AssignedToUser was always non-null. However, AssignedToUserId is nullable by design, because incidents may be created before assignment.

## Fix
Updated the mapper to check whether AssignedToUser is null before creating the AssignedTo response object.

## Verification
- Assigned incident returned 200.
- Unassigned incident returned 200 with assignedTo = null.
- Missing incident returned 404.

## Prevention
Added explicit nullable response handling for optional relationships and documented expected behavior for unassigned incidents.
