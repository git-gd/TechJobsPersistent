--Part 1
--list the columns and their data types in the Jobs table.
--
--EmployerId	int
--Id			int
--Name		longtext
--*** Part 2
--write a query to list the names of the employers in St. Louis City.
--
SELECT e.Name
FROM employers e
WHERE e.Location LIKE ("St. Louis City");
--*** Part 3
--write a query to return a list of the names and descriptions of all skills
--that are attached to jobs in alphabetical order. If a skill does not have a job listed,
--it should not be included in the results of this query.
--
--SELECT s.Name, s.Description
--FROM skills s
--JOIN jobskills j ON s.Id = j.SkillId
--GROUP BY s.Name
--ORDER BY s.Name ASC;
-- You will need to make use of �is not null�. ...???
