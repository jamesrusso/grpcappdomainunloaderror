# grpcappdomainunloaderror
Test Solution to show issue with loading GRPC in a separate application domain and not being able to unload it.


- setup a pubsub subscription
- setup a service account which has access to this pubsub subscription
- adjust the topic and subscription name in Plugin/Plugin.cs
- build the solution
- run the solution

It will error out when attempting to delete the plugin directory indicating that the 'grpc_csharp_ext.x86.dll' cannot be deleted. I

It may also warn you about the pdb file, but you can avoid this if you run it not from visual studio, but rather from the command line.
