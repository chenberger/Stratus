import Head from 'next/head';
import ArrowUpOnSquareIcon from '@heroicons/react/24/solid/ArrowUpOnSquareIcon';
import ArrowDownOnSquareIcon from '@heroicons/react/24/solid/ArrowDownOnSquareIcon';
import PlusIcon from '@heroicons/react/24/solid/PlusIcon';
import React, { useState, useEffect  } from 'react';
import axios from 'axios';
import {
  Box,
  Button,
  Container,
  Pagination,
  Stack,
  SvgIcon,
  Typography,
  Unstable_Grid2 as Grid
} from '@mui/material';
import { Layout as DashboardLayout } from 'src/layouts/dashboard/layout';
import { CustomMachineCard } from 'src/sections/CustomMachines/custom-machine-card';
import { CustomMachinesSearch } from 'src/sections/CustomMachines/custom-machines-search';

const Page = () => {
  const [machines, setMachines] = useState([]);

  useEffect(() => {
    axios.get('https://localhost:7094/GetRecommendationsInstances')
    .then(response => {
      const data = response.data.data;
      console.log("custom: ", data);
      setMachines(data);

    })
    .catch(error => console.error(error));
    },[]);

    const [currentPage, setCurrentPage] = useState(1);

    const handlePageChange = (event, page) => {
  setCurrentPage(page);
};

const startIndex = (currentPage - 1) * 6;
const endIndex = startIndex + 6;
    return (
      <>
  
    <Head>
      <title>
        Recommendations | STRATUS
      </title>
    </Head>
    <Box
      component="main"
      sx={{
        flexGrow: 1,
        py: 8
      }}
    >
      <Container maxWidth="xl">
        <Stack spacing={3}>
          <Stack
            direction="row"
            justifyContent="space-between"
            spacing={4}
          >
            <Stack spacing={1}>
              <Typography variant="h4">
              Recommendations
              </Typography>
              <Stack
                alignItems="center"
                direction="row"
                spacing={1}
              >
                <Button
                  color="inherit"
                  startIcon={(
                    <SvgIcon fontSize="small">
                      <ArrowUpOnSquareIcon />
                    </SvgIcon>
                  )}
                >
                  Import
                </Button>
                <Button
                  color="inherit"
                  startIcon={(
                    <SvgIcon fontSize="small">
                      <ArrowDownOnSquareIcon />
                    </SvgIcon>
                  )}
                >
                  Export
                </Button>
              </Stack>
            </Stack>
            <div>
              <Button
                startIcon={(
                  <SvgIcon fontSize="small">
                    <PlusIcon />
                  </SvgIcon>
                )}
                variant="contained"
              >
                Add
              </Button>
            </div>
          </Stack>
          <CustomMachinesSearch />
          <Grid container spacing={3}>
              {machines.slice(startIndex, endIndex).map((machine) => (
                <Grid xs={12} md={6} lg={4}>
                  {machine ? (
                    <CustomMachineCard machine={machine} />
                  ) : (
                    <div>Loading...</div>
                  )}
                </Grid>
              ))}
            </Grid>
            <Box sx={{ display: 'flex', justifyContent: 'center' }}>
              <Pagination
                count={Math.ceil(machines.length / 6)}
                page={currentPage}
                onChange={handlePageChange}
                size="small"
              />
            </Box>
        </Stack>
      </Container>
    </Box>
  </>
);
          }

Page.getLayout = (page) => (
  <DashboardLayout>
    {page}
  </DashboardLayout>
);

export default Page;
