# PulseService

PulseService is a ASP.NET Web API for Pulse, a discussion app that allows users to debate various topics by selecting one of an array of pre-defined opinions.

## Features
- A REST API for allowing the front-end application to perform CRUD operations on data
- A database adapter for MongoDB
- A clean hexagonal architecture
- JWT-based security for authorisation

### Stage 1 (MVP) (Complete)
- Route for creating user accounts
- Route for getting JWT for subsequent authentication
- Routes for getting/creating/deleting Pulses
- Route for updating votes on Pulses (including removing a vote)

### Stage 2
- Route for getting Pulses for a certain date range
- Route for getting a single Pulse by ID
- Route for getting all Pulses created by a user
- Backend logic and data structures for discussions!
